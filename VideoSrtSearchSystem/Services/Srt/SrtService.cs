using SubtitlesParser.Classes.Parsers;
using System.Text;
using VideoSrtSearchSystem.Config;
using VideoSrtSearchSystem.DTO.Request.Srt;
using VideoSrtSearchSystem.DTO.Response.Srt;
using VideoSrtSearchSystem.Exceptions;
using VideoSrtSearchSystem.Models.LiveStraming;
using VideoSrtSearchSystem.Repository.LiveStraming;
using VideoSrtSearchSystem.Repository.Srt;
using VideoSrtSearchSystem.Tool;
using VideoSrtSearchSystem.Tool.MySQL;

namespace VideoSrtSearchSystem.Services.Srt
{
    public class SrtService(
        ILiveStreamingRepository _liveStreamingRepository,
        ILiveStreamingSrtRepository _liveStreamingSrtRepository,
        IMySQLConnectionProvider _mySQLConnectionProvider,
        ILogger<SrtService> _logger,
        ICommonTool _commonTool,
        IWebHostEnvironment appEnvironment
    ) : ISrtService
    {
        private readonly string outputDirectory = Path.Combine(appEnvironment.WebRootPath, "output");
        private readonly int _searchPageSize = 20;

        public void ImportSrt(ImportSrtRequest request)
        {
            try
            {
                if (!File.Exists(request.SrtPath))
                {
                    throw new MyException(ResponseCode.FILE_NOT_FOUND);
                }
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                // 檢查影片是否已存在
                var liveStramingModel = _liveStreamingRepository.GetByUrl(request.VideoUrl, connection);
                bool noVideo = string.IsNullOrEmpty(liveStramingModel.ls_guid);
                // 影片 Guid
                var videoGuid = noVideo ?
                    Guid.NewGuid().ToString() :
                    liveStramingModel.ls_guid;

                var parser = new SrtParser();
                using (var fileStream = File.OpenRead(request.SrtPath))
                {
                    var items = parser.ParseStream(fileStream, Encoding.UTF8);
                    uint index = 0;
                    var insertSrtList = items.Select(item => new LiveStreamingSrtModel
                    {
                        lss_ls_id = liveStramingModel.ls_id,
                        lss_num = index++,
                        lss_start = TimeSpan.FromMilliseconds(item.StartTime).ToString(@"hh\:mm\:ss\.fff"),
                        lss_end = TimeSpan.FromMilliseconds(item.EndTime).ToString(@"hh\:mm\:ss\.fff"),
                        lss_text = string.Join(" ", item.PlaintextLines),
                    }).ToList();
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
                            });
                            insertSrtList.ForEach(item => item.lss_ls_id = liveStramingModel.ls_id);
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
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

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
                        SrtStartTimeSeconds = (int)TimeSpan.Parse(srtModel.ModelV!.lss_start).TotalSeconds,
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
