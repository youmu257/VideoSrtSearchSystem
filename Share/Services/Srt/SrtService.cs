using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Share.Config.Appsettings;
using Share.Const;
using Share.DTO.Request.Srt;
using Share.DTO.Request.Tag;
using Share.DTO.Response.Srt;
using Share.Models.LiveStraming;
using Share.Repositorys.LiveStraming;
using Share.Repositorys.Srt;
using Share.Repositorys.Tag;
using Share.Services.Tag;
using Share.Tool;
using Share.Tool.MySQL;
using SubtitlesParser.Classes.Parsers;
using System.Text;

namespace Share.Services.Srt
{
    /// <summary>
    /// 字幕服務實作 - 負責字幕的匯入、搜尋、下載和快取管理
    /// </summary>
    public class SrtService(
        ITagService _tagService,
        ILiveStreamingRepository _liveStreamingRepository,
        ILiveStreamingSrtRepository _liveStreamingSrtRepository,
        ILiveStreamingTagMappingRepository _liveStreamingTagMappingRepository,
        IMySQLConnectionProvider _mySQLConnectionProvider,
        ILogger<SrtService> _logger,
        ICommonTool _commonTool,
        IOptions<SrtConfig> _srtConfig
    ) : ISrtService
    {
        /// <summary>
        /// 搜尋結果每頁顯示的數量
        /// </summary>
        private readonly int _searchPageSize = 20;
        
        /// <summary>
        /// 字幕檔案預設儲存路徑
        /// </summary>
        private readonly string _srtDefaultPath = _srtConfig.Value.SrtDefaultPath;
        
        /// <summary>
        /// 字幕時間格式（時:分:秒,毫秒）
        /// </summary>
        private readonly string _srtTimeFormat = @"hh\:mm\:ss\,fff";
        
        /// <summary>
        /// 快取所有影片字幕列表（用於記憶體搜尋）
        /// </summary>
        private static List<LiveStreamingModel> _cacheAllSrtList = new List<LiveStreamingModel>();
        
        /// <summary>
        /// 快取影片字典（以影片 ID 為鍵）
        /// </summary>
        private static Dictionary<LsId, LiveStreamingModel> _cacheVideoDict = new Dictionary<LsId, LiveStreamingModel>();

        /// <summary>
        /// 匯入字幕檔案到資料庫
        /// </summary>
        /// <param name="request">匯入字幕請求</param>
        /// <returns>成功回傳 SUCCESS，失敗回傳影片 URL</returns>
        public string ImportSrt(ImportSrtRequest request)
        {
            try
            {
                // 如果未指定字幕路徑，使用預設路徑
                SetDefaultSrtPathIfEmpty(request);

                // 插入標籤並取得標籤 ID 列表
                var tagIdList = InsertTagsAndGetIds(request.TagList);
                
                // 從 YouTube URL 提取影片 ID
                request.VideoUrl = ExtractVideoId(request.VideoUrl);

                // 建立資料庫連線
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                
                // 檢查影片是否已存在
                var liveModel = _liveStreamingRepository.GetByUrl(request.VideoUrl, connection);
                bool isNewVideo = string.IsNullOrWhiteSpace(liveModel.ls_guid);
                var videoGuid = isNewVideo ? Guid.NewGuid().ToString() : liveModel.ls_guid;

                // 解析字幕檔案
                var insertSrtList = ParseSrtFile(request.SrtPath, liveModel.ls_id);

                // 新影片才需要合併所有字幕文字（用於全文搜尋）
                var allSrt = isNewVideo ? string.Join("", insertSrtList.Select(item => item.lss_text).Distinct()) : string.Empty;

                // 開始資料庫交易
                using var trans = connection.BeginTransaction();
                try
                {
                    if (isNewVideo)
                    {
                        // 新增影片資訊到資料庫
                        liveModel.ls_id = _liveStreamingRepository.Insert(connection, trans, new LiveStreamingModel
                        {
                            ls_guid = videoGuid,
                            ls_title = request.VideoTitle,
                            ls_url = request.VideoUrl,
                            ls_livetime = DateTime.Parse(request.LiveTime),
                            ls_all_srt = allSrt,
                        });
                        
                        // 更新字幕列表的影片 ID
                        insertSrtList.ForEach(item => item.lss_ls_id = liveModel.ls_id);
                    }
                    else
                    {
                        // 影片已存在，清除舊的字幕資料
                        _liveStreamingSrtRepository.DeleteByVideoId(connection, trans, liveModel.ls_id);
                        
                        // 更新影片資訊和合併字幕
                        _liveStreamingRepository.UpdateAllSrt(connection, trans, videoGuid, request.VideoUrl, request.VideoTitle, allSrt);
                        
                        // 清除舊的標籤關聯
                        _liveStreamingTagMappingRepository.Delete(connection, trans, liveModel.ls_id);
                    }
                    
                    // 插入新的字幕資料
                    if (insertSrtList.Count > 0)
                    {
                        _liveStreamingSrtRepository.Insert(connection, trans, insertSrtList);
                    }
                    
                    // 插入標籤關聯
                    _liveStreamingTagMappingRepository.Insert(connection, trans, liveModel.ls_id, tagIdList);

                    // 提交交易
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "匯入字幕時發生錯誤: {VideoUrl}", request.VideoUrl);
                    trans.Rollback();
                    return request.VideoUrl;
                }
                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "匯入字幕時發生異常");
                throw;
            }
        }

        /// <summary>
        /// 從資料庫搜尋包含關鍵字的字幕
        /// </summary>
        /// <param name="keyword">搜尋關鍵字</param>
        /// <param name="page">頁碼（從 1 開始）</param>
        /// <returns>字幕搜尋結果</returns>
        public SearchSrtResponse SearchSrt(string keyword, int page)
        {
            try
            {
                // 建立資料庫連線
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                
                // 從資料庫查詢包含關鍵字的字幕
                var srtList = _liveStreamingSrtRepository.GetByLikeKeyword(keyword, page, _searchPageSize, connection);
                
                // 使用字典整理字幕資料（以影片 GUID 分組）
                var srtDict = new Dictionary<string, SearchSrtVideoResponse>();
                
                // 遍歷字幕列表，組織成影片分組的結構
                foreach (var srtModel in srtList)
                {
                    // 移除 GUID 中的連字符
                    string videoGuid = srtModel.ModelK!.ls_guid.Replace("-", "");
                    
                    // 建立字幕資料物件
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
                            SrtList = new List<SrtResponse> { srtData }
                        });
                        continue;
                    }
                    
                    // 將字幕加入該影片的字幕列表
                    srtDict[videoGuid].SrtList.Add(srtData);
                }
                
                // 取得查詢總數量（用於計算總頁數）
                var totalCount = _liveStreamingSrtRepository.GetTotalPageByLikeKeyword(keyword, connection);
                // 建構回傳結果
                return new SearchSrtResponse
                {
                    TotalPage = _commonTool.GetTotalPage(totalCount, _searchPageSize),
                    VideoList = srtDict.Values.ToList(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "從資料庫搜尋字幕時發生錯誤: {Keyword}", keyword);
                throw;
            }
        }

        /// <summary>
        /// 從記憶體快取搜尋包含關鍵字的字幕（支援日期範圍篩選）
        /// </summary>
        /// <param name="request">搜尋請求</param>
        /// <returns>字幕搜尋結果</returns>
        public SearchSrtResponse SearchSrtByMemory(SearchSrtRequest request)
        {
            try
            {
                // 建立資料庫連線
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                
                // 初始化快取（如果尚未載入）
                if (_cacheAllSrtList.Count == 0)
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
                
                // 從快取中篩選包含關鍵字且在日期範圍內的影片
                var keywordInVideoList = _cacheAllSrtList.Where(item =>
                    item.ls_all_srt.Contains(request.Keyword) &&
                    _commonTool.CheckInTimeRage(item.ls_livetime, request.Start, request.End)
                );

                // 取得當前頁面的影片 ID 列表
                var videoIdList = keywordInVideoList
                    .Select(item => item.ls_id)
                    .Skip((request.Page - 1) * _searchPageSize)
                    .Take(_searchPageSize)
                    .ToList();
                
                // 計算符合條件的總影片數
                var totalCount = keywordInVideoList.Count();
                
                // 從資料庫取得指定影片的詳細字幕資料
                var srtList = _liveStreamingSrtRepository.GetByLikeKeyword(videoIdList, request.Keyword, connection);
                
                // 使用字典整理字幕資料（以影片 GUID 分組）
                var srtDict = new Dictionary<string, SearchSrtVideoResponse>();
                
                // 遍歷字幕列表，組織成影片分組的結構
                foreach (var srtModel in srtList)
                {
                    // 從快取字典取得影片資訊
                    string videoGuid = _cacheVideoDict[srtModel.lss_ls_id].ls_guid;
                    
                    // 建立字幕資料物件
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
                            SrtList = new List<SrtResponse> { srtData }
                        });
                        continue;
                    }
                    
                    // 將字幕加入該影片的字幕列表
                    srtDict[videoGuid].SrtList.Add(srtData);
                }

                // 建構回傳結果
                return new SearchSrtResponse
                {
                    TotalPage = _commonTool.GetTotalPage(totalCount, _searchPageSize),
                    VideoList = srtDict.Values.ToList(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "從記憶體搜尋字幕時發生錯誤: {Keyword}", request.Keyword);
                throw;
            }
        }

        /// <summary>
        /// 下載指定影片的字幕檔案
        /// </summary>
        /// <param name="videoGuid">影片 GUID</param>
        /// <returns>字幕檔案資料流和檔名</returns>
        public DownloadSrtResponse DownloadSrt(string videoGuid)
        {
            try
            {
                // 建立資料庫連線
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                
                // 取得影片資訊
                var videoModel = _liveStreamingRepository.GetByGuid(videoGuid, connection);
                
                // 取得該影片的所有字幕
                var srtList = _liveStreamingSrtRepository.GetByVideoId(videoModel.ls_id, connection);
                
                // 組合成標準 SRT 格式
                var sb = new StringBuilder();
                foreach (var srtObj in srtList)
                {
                    sb = sb.Append($"{srtObj.lss_num}\n")
                        .Append($"{srtObj.lss_start} --> {srtObj.lss_end}\n")
                        .Append($"{srtObj.lss_text}\n\n");
                }
                
                // 將字串轉換為 UTF-8 字節數組
                byte[] byteArray = Encoding.UTF8.GetBytes(sb.ToString());

                // 建立 MemoryStream 供下載使用
                return new DownloadSrtResponse
                {
                    FileName = $"{videoModel.ls_title}.srt",
                    SrtFile = new MemoryStream(byteArray),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "下載字幕時發生錯誤: {VideoGuid}", videoGuid);
                throw;
            }
        }

        /// <summary>
        /// 設定預設字幕路徑（如果請求中未指定）
        /// </summary>
        /// <param name="request">匯入字幕請求</param>
        private void SetDefaultSrtPathIfEmpty(ImportSrtRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.SrtPath))
            {
                request.SrtPath = Path.Combine(_srtDefaultPath, $"{request.VideoTitle}.srt");
            }
        }

        /// <summary>
        /// 插入標籤並取得標籤 ID 列表
        /// </summary>
        /// <param name="tagList">標籤列表</param>
        /// <returns>標籤 ID 列表</returns>
        private List<LstId> InsertTagsAndGetIds(List<TagDTO> tagList)
        {
            var tagIdList = new List<LstId>();
            foreach (var tag in tagList)
            {
                var tagId = _tagService.InsertTag(new AddTagRequest
                {
                    TagType = tag.TagType,
                    TagName = tag.TagName,
                });
                tagIdList.Add(LstId.From(tagId));
            }
            return tagIdList;
        }

        /// <summary>
        /// 從 YouTube URL 提取影片 ID
        /// </summary>
        /// <param name="url">YouTube 影片 URL</param>
        /// <returns>影片 ID</returns>
        private string ExtractVideoId(string url)
        {
            return url.Replace("https://www.youtube.com/watch?v=", "");
        }

        /// <summary>
        /// 解析字幕檔案並轉換為資料模型列表
        /// </summary>
        /// <param name="srtPath">字幕檔案路徑</param>
        /// <param name="videoId">影片 ID</param>
        /// <returns>字幕資料模型列表</returns>
        private List<LiveStreamingSrtModel> ParseSrtFile(string srtPath, LsId videoId)
        {
            // 檢查檔案是否存在
            if (!File.Exists(srtPath))
            {
                return new List<LiveStreamingSrtModel>();
            }

            // 使用 SRT 解析器解析字幕檔案
            var parser = new SrtParser();
            var insertSrtList = new List<LiveStreamingSrtModel>();
            
            using var fileStream = File.OpenRead(srtPath);
            var items = parser.ParseStream(fileStream, Encoding.UTF8);
            
            // 遍歷解析結果，轉換為資料模型
            uint index = 1;
            foreach (var item in items)
            {
                // 將多行文字合併為單行
                var srtText = string.Join(" ", item.PlaintextLines);
                
                insertSrtList.Add(new LiveStreamingSrtModel
                {
                    lss_ls_id = videoId,
                    lss_num = index++,
                    lss_start = TimeSpan.FromMilliseconds(item.StartTime).ToString(_srtTimeFormat),
                    lss_end = TimeSpan.FromMilliseconds(item.EndTime).ToString(_srtTimeFormat),
                    lss_text = srtText,
                });
            }
            
            return insertSrtList;
        }
    }
}
