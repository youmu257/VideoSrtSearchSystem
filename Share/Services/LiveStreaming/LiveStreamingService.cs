using Microsoft.Extensions.Logging;
using Share.Const;
using Share.DTO.Request;
using Share.DTO.Request.Tag;
using Share.DTO.Request.Video;
using Share.DTO.Response.Tag;
using Share.DTO.Response.Video;
using Share.Exceptions;
using Share.Models.LiveStraming;
using Share.Repositorys.LiveStraming;
using Share.Repositorys.Tag;
using Share.Services.Tag;
using Share.Tool;
using Share.Tool.MySQL;

namespace Share.Services.Video
{
    /// <summary>
    /// 直播影片服務實作 - 負責影片資料的查詢、編輯和標籤管理
    /// </summary>
    public class LiveStreamingService(
        ITagService _tagService,
        ILiveStreamingRepository _liveStreamingRepository,
        ILiveStreamingTagRepository _liveStreamingTagRepository,
        ILiveStreamingTagTypeRepository _liveStreamingTagTypeRepository,
        ILiveStreamingTagMappingRepository _liveStreamingTagMappingRepository,
        IMySQLConnectionProvider _mySQLConnectionProvider,
        ICommonTool _commonTool,
        ILogger<LiveStreamingService> _logger
    ) : ILiveStreamingService
    {
        /// <summary>
        /// 每頁顯示的影片數量
        /// </summary>
        private static readonly int pageSize = 25;

        /// <summary>
        /// 取得所有影片列表（支援關鍵字搜尋和分頁）
        /// </summary>
        /// <param name="request">搜尋請求，包含關鍵字和頁碼</param>
        /// <returns>影片列表和總頁數</returns>
        public GetAllVideoResponse GetAllVideo(SearchRequest request)
        {
            try
            {
                // 建立資料庫連線
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                
                // 取得影片列表（頁碼從 1 開始，轉換為從 0 開始的索引）
                var liveStramingList = _liveStreamingRepository.GetAll(
                    request.Keyword, 
                    request.Page - 1, 
                    pageSize, 
                    connection);
                
                // 取得符合條件的總影片數
                var totalCount = _liveStreamingRepository.GetCount(request.Keyword, connection);
                
                // 建構回傳物件
                return new GetAllVideoResponse
                {
                    TotalPage = _commonTool.GetTotalPage(totalCount, pageSize),
                    VideoList = liveStramingList.Select(item => new VideoResponse
                    {
                        VideoGuid = item.ls_guid,
                        VideoTitle = item.ls_title,
                        VideoUrl = item.ls_url,
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得影片列表時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 取得單一影片的詳細資訊
        /// </summary>
        /// <param name="guid">影片 GUID</param>
        /// <returns>影片詳細資訊，包含標題、URL、標籤列表和標籤類型列表</returns>
        public GetOneVideoInfoResponse GetOneVideoInfo(string guid)
        {
            try
            {
                // 建立資料庫連線
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                
                // 取得影片基本資訊
                var liveStramingModel = _liveStreamingRepository.GetByGuid(guid, connection);
                
                // 取得該影片的所有標籤
                var tagList = _liveStreamingTagRepository.GetByVideoGuid(guid, connection);
                
                // 取得所有標籤類型列表（用於下拉選單）
                var tagTypeList = _liveStreamingTagTypeRepository.GetAll(connection);

                return new GetOneVideoInfoResponse
                {
                    VideoGuid = guid,
                    VideoTitle = liveStramingModel.ls_title,
                    VideoUrl = liveStramingModel.ls_url,
                    TagList = tagList.Select(item => new TagEditDTO
                    {
                        Id = item.lst_id,
                        TagName = item.lst_name,
                        TagType = item.lst_type,
                    }).ToList(),
                    TagTypeList = tagTypeList.Select(item => new TagTypeResponse
                    {
                        Id = item.lstt_type,
                        TypeName = item.lstt_name,
                    }).ToList(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得影片詳細資訊時發生錯誤: {Guid}", guid);
                throw;
            }
        }

        /// <summary>
        /// 更新影片資訊（包含標題和標籤）
        /// </summary>
        /// <param name="request">編輯影片請求，包含影片 GUID、新標題和標籤列表</param>
        /// <exception cref="MyException">當影片不存在時拋出</exception>
        public void UpdateVideoInfo(EditVideoRequest request)
        {
            try
            {
                // 建立資料庫連線
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                
                // 檢查影片是否存在
                var liveStramingModel = _liveStreamingRepository.GetByGuid(request.VideoGuid, connection);
                if (liveStramingModel.ls_id.Value == 0)
                {
                    throw new MyException(ResponseCode.VIDEO_NOT_EXIST);
                }
                
                // 如果標題沒有變更，則設為空字串（不需更新）
                if (liveStramingModel.ls_title == request.VideoTitle)
                {
                    request.VideoTitle = string.Empty;
                }
                
                #region 分析標籤變更
                // 取得當前影片的所有標籤
                var tagList = _liveStreamingTagRepository.GetByVideoGuid(request.VideoGuid, connection);
                var tagIdList = tagList.Select(item => item.lst_id).ToList();
                
                // 取得請求中的現有標籤 ID（ID 不為 0 表示是現有標籤）
                var needCheckTagIdList = request.TagList
                    .Where(item => item.Id.Value != 0)
                    .Select(item => item.Id)
                    .ToList();
                
                // 計算需要刪除的標籤（原有但請求中沒有的）
                var deleteTagIdList = tagIdList.Except(needCheckTagIdList);
                
                // 取得需要新增的標籤（ID 為 0 表示是新標籤）
                var newTagList = request.TagList.Where(item => item.Id.Value == 0).ToList();
                var newTagIdList = new List<LstId>();
                
                // 逐一新增標籤
                foreach (var tagData in newTagList)
                {
                    var tagId = _tagService.InsertTag(new AddTagRequest
                    {
                        TagType = tagData.TagType,
                        TagName = tagData.TagName,
                    });
                    newTagIdList.Add(LstId.From(tagId));
                }
                #endregion
                
                #region 執行資料庫更新操作（使用交易確保一致性）
                // 開始資料庫交易
                var trans = connection.BeginTransaction();
                if (!string.IsNullOrEmpty(request.VideoTitle))
                {
                    _liveStreamingRepository.UpdateTitle(connection, trans, request.VideoGuid, request.VideoTitle);
                }
                foreach (var deleteId in deleteTagIdList)
                {
                    _liveStreamingTagMappingRepository.Delete(connection, trans, liveStramingModel.ls_id, deleteId);
                }
                if (newTagIdList.Count > 0)
                {
                    _liveStreamingTagMappingRepository.Insert(connection, trans, liveStramingModel.ls_id, newTagIdList);
                }
                trans.Commit();
                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新影片資訊時發生錯誤: {VideoGuid}", request.VideoGuid);
                throw;
            }
        }
    }
}
