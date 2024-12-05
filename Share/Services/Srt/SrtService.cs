using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Share.Config.Appsettings;
using Share.Const;
using Share.DTO.Request.Srt;
using Share.DTO.Response.Srt;
using Share.Models.LiveStraming;
using Share.Repositorys.LiveStraming;
using Share.Repositorys.Srt;
using Share.Tool;
using Share.Tool.MySQL;
using SubtitlesParser.Classes.Parsers;
using System.Text;

namespace Share.Services.Srt
{
    public class SrtService(
        ILiveStreamingRepository _liveStreamingRepository,
        ILiveStreamingSrtRepository _liveStreamingSrtRepository,
        IMySQLConnectionProvider _mySQLConnectionProvider,
        ILogger<SrtService> _logger,
        ICommonTool _commonTool,
        IOptions<SrtConfig> _srtConfig,
        IWebHostEnvironment appEnvironment
    ) : ISrtService
    {
        private readonly string outputDirectory = Path.Combine(appEnvironment.WebRootPath, "output");
        private readonly int _searchPageSize = 20;
        private readonly string _srtDefaultPath = _srtConfig.Value.SrtDefaultPath;
        private readonly string _srtTimeFormat = @"hh\:mm\:ss\,fff";
        private static List<LiveStreamingModel> _cacheAllSrtList = new List<LiveStreamingModel>();
        private static Dictionary<LsId, LiveStreamingModel> _cacheVideoDict = new Dictionary<LsId, LiveStreamingModel>();

        public string ImportSrt(ImportSrtRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.SrtPath))
                {
                    request.SrtPath = Path.Combine(_srtDefaultPath, $"{request.VideoTitle}.srt");
                }
                if (!File.Exists(request.SrtPath))
                {
                    return ResponseCode.FILE_NOT_FOUND;
                }
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                // 只留影片ID
                request.VideoUrl = request.VideoUrl.Replace("https://www.youtube.com/watch?v=", "");
                // 檢查影片是否已存在
                var liveStramingModel = _liveStreamingRepository.GetByUrl(request.VideoUrl, connection);
                bool noVideo = string.IsNullOrEmpty(liveStramingModel.ls_guid);
                // 影片 Guid
                var videoGuid = noVideo ?
                    Guid.NewGuid().ToString() :
                    liveStramingModel.ls_guid;
                #region 整理寫入的資料
                var insertSrtList = new List<LiveStreamingSrtModel>();
                var parser = new SrtParser();
                using (var fileStream = File.OpenRead(request.SrtPath))
                {
                    var items = parser.ParseStream(fileStream, Encoding.UTF8);
                    uint index = 1;
                    foreach (var item in items)
                    {
                        var srtText = string.Join(" ", item.PlaintextLines);
                        insertSrtList.Add(new LiveStreamingSrtModel
                        {
                            lss_ls_id = liveStramingModel.ls_id,
                            lss_num = index++,
                            lss_start = TimeSpan.FromMilliseconds(item.StartTime).ToString(@"hh\:mm\:ss\,fff"),
                            lss_end = TimeSpan.FromMilliseconds(item.EndTime).ToString(@"hh\:mm\:ss\,fff"),
                            lss_text = srtText,
                        });
                    }
                }
                if (insertSrtList.Count() == 0)
                {
                    return ResponseCode.SUCCESS;
                }
                var allSrt = string.Join("", insertSrtList.Select(item => item.lss_text).Distinct());
                #endregion
                var trans = connection.BeginTransaction();
                try
                {
                    if (noVideo)
                    {
                        // 寫入影片資訊
                        liveStramingModel.ls_id = _liveStreamingRepository.Insert(connection, trans, new LiveStreamingModel
                        {
                            ls_guid = videoGuid,
                            ls_title = request.VideoTitle,
                            ls_url = request.VideoUrl,
                            ls_livetime = DateTime.Parse(request.LiveTime),
                            ls_all_srt = allSrt,
                        });
                        insertSrtList.ForEach(item => item.lss_ls_id = liveStramingModel.ls_id);
                    }
                    else
                    {
                        // 已有影片資料，把原有字幕清除
                        _liveStreamingSrtRepository.DeleteByVideoId(connection, trans, liveStramingModel.ls_id);
                        // 更新全部字幕
                        _liveStreamingRepository.UpdateAllSrt(connection, trans, videoGuid, allSrt);
                    }
                    // 寫入字幕資訊
                    _liveStreamingSrtRepository.Insert(connection, trans, insertSrtList);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    trans.Rollback();
                }
                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 從DB中撈關鍵字出現在哪幾個影片中，再抓對應的字幕出來
        /// </summary>
        public SearchSrtResponse SearchSrt(string keyword, int page)
        {
            try
            {
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                // 查詢影片字幕
                var srtList = _liveStreamingSrtRepository.GetByLikeKeyword(keyword, page, _searchPageSize, connection);
                var srtDict = new Dictionary<string, SearchSrtVideoResponse>();
                foreach (var srtModel in srtList)
                {
                    string videoGuid = srtModel.ModelK!.ls_guid.Replace("-", "");
                    var srtData = new SrtResponse
                    {
                        Context = srtModel.ModelV!.lss_text,
                        SrtStartTimeSeconds = (int)TimeSpan.ParseExact(srtModel.ModelV!.lss_start, _srtTimeFormat, null).TotalSeconds,
                        SrtStartTime = srtModel.ModelV!.lss_start,
                        SrtEndTime = srtModel.ModelV!.lss_end,
                    };
                    if (srtDict.ContainsKey(videoGuid) == false)
                    {
                        srtDict.Add(videoGuid, new SearchSrtVideoResponse
                        {
                            VideoTitle = srtModel.ModelK!.ls_title,
                            VideoGuid = videoGuid,
                            VideoUrl = srtModel.ModelK!.ls_url,
                            LiveTime = srtModel.ModelK!.ls_livetime.ToString("yyyy-MM-dd"),
                            SrtList = new List<SrtResponse>
                            {
                                srtData
                            }
                        });
                        continue;
                    }
                    srtDict[videoGuid].SrtList.Add(srtData);
                }
                // 取得查詢總數量
                var totalCount = _liveStreamingSrtRepository.GetTotalPageByLikeKeyword(keyword, connection);
                return new SearchSrtResponse
                {
                    TotalPage = _commonTool.GetTotalPage(totalCount, _searchPageSize),
                    VideoList = srtDict.Values.ToList(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 從記憶體中抓關鍵字在哪幾個影片中，去從DB中抓對應字幕
        /// </summary>
        public SearchSrtResponse SearchSrtByMemory(string keyword, int page)
        {
            try
            {
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                // 字幕檔未載入就初始化
                if (_cacheAllSrtList.Count() == 0)
                {
                    _cacheAllSrtList = _liveStreamingRepository.GetAll(connection);
                    _cacheVideoDict = _cacheAllSrtList.ToDictionary(item => item.ls_id, item => new LiveStreamingModel
                    {
                        ls_guid = item.ls_guid.Replace("-", ""),
                        ls_title = item.ls_title,
                        ls_url = item.ls_url,
                        ls_livetime = item.ls_livetime,
                    });
                }
                var keywordInVideoList = _cacheAllSrtList.Where(item => item.ls_all_srt.Contains(keyword));
                var videoIdList = keywordInVideoList.Select(item => item.ls_id).Skip((page-1)* _searchPageSize).Take(_searchPageSize).ToList();
                // 取得查詢總數量
                var totalCount = keywordInVideoList.Count();
                // 查詢影片字幕
                var srtList = _liveStreamingSrtRepository.GetByLikeKeyword(videoIdList, keyword, connection);
                var srtDict = new Dictionary<string, SearchSrtVideoResponse>();
                foreach (var srtModel in srtList)
                {
                    string videoGuid = _cacheVideoDict[srtModel.lss_ls_id].ls_guid;
                    var srtData = new SrtResponse
                    {
                        Context = srtModel.lss_text,
                        SrtStartTimeSeconds = (int)TimeSpan.ParseExact(srtModel.lss_start, _srtTimeFormat, null).TotalSeconds,
                        SrtStartTime = srtModel.lss_start,
                        SrtEndTime = srtModel.lss_end,
                    };
                    if (srtDict.ContainsKey(videoGuid) == false)
                    {
                        srtDict.Add(videoGuid, new SearchSrtVideoResponse
                        {
                            VideoTitle = _cacheVideoDict[srtModel.lss_ls_id].ls_title,
                            VideoGuid = videoGuid,
                            VideoUrl = _cacheVideoDict[srtModel.lss_ls_id].ls_url,
                            LiveTime = _cacheVideoDict[srtModel.lss_ls_id].ls_livetime.ToString("yyyy-MM-dd"),
                            SrtList = new List<SrtResponse>
                            {
                                srtData
                            }
                        });
                        continue;
                    }
                    srtDict[videoGuid].SrtList.Add(srtData);
                }

                return new SearchSrtResponse
                {
                    TotalPage = _commonTool.GetTotalPage(totalCount, _searchPageSize),
                    VideoList = srtDict.Values.ToList(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        public DownloadSrtResponse DownloadSrt(string videoGuid)
        {
            try
            {
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                // 取得影片資訊
                var videoModel = _liveStreamingRepository.GetByGuid(videoGuid, connection);
                // 取得影片字幕
                var srtList = _liveStreamingSrtRepository.GetByVideoId(videoModel.ls_id, connection);
                var sb = new StringBuilder();
                foreach (var srtObj in srtList)
                {
                    sb = sb.Append($"{srtObj.lss_num}\n")
                        .Append($"{srtObj.lss_start} --> {srtObj.lss_end}\n")
                        .Append($"{srtObj.lss_text}\n\n");
                }
                // 將字串轉換為字節數組
                byte[] byteArray = Encoding.UTF8.GetBytes(sb.ToString());

                // 將字節數組轉換為 MemoryStream
                return new DownloadSrtResponse
                {
                    FileName = $"{videoModel.ls_title}.srt",
                    SrtFile = new MemoryStream(byteArray),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
    }
}
