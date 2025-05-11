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
        private static readonly int pageSize = 25;

        public GetAllVideoResponse GetAllVideo(SearchRequest request)
        {
            try
            {
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                // 取得影片列表
                var liveStramingList = _liveStreamingRepository.GetAll(request.Keyword, request.Page - 1, pageSize, connection);
                var totalCount = _liveStreamingRepository.GetCount(request.Keyword, connection);
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
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        public GetOneVideoInfoResponse GetOneVideoInfo(string guid)
        {
            try
            {
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                // 取得影片資訊
                var liveStramingModel = _liveStreamingRepository.GetByGuid(guid, connection);
                // 取得影片標籤
                var tagList = _liveStreamingTagRepository.GetByVideoGuid(guid, connection);
                // 取得標邊類型列表
                var tagTypeList = _liveStreamingTagTypeRepository.GetAll(connection);
                var tagTypeDict = tagTypeList.ToDictionary(item => item.lstt_type, item => item.lstt_name);
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
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        public void UpdateVideoInfo(EditVideoRequest request)
        {
            try
            {
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                // 檢查影片資訊
                var liveStramingModel = _liveStreamingRepository.GetByGuid(request.VideoGuid, connection);
                if (liveStramingModel.ls_id.Value == 0)
                {
                    throw new MyException(ResponseCode.VIDEO_NOT_EXIST);
                }
                if (liveStramingModel.ls_title == request.VideoTitle)
                {
                    request.VideoTitle = string.Empty;
                }
                #region 檢查影片標籤
                var tagList = _liveStreamingTagRepository.GetByVideoGuid(request.VideoGuid, connection);
                var tagIdList = tagList.Select(item => item.lst_id).ToList();
                var needCheckTagIdList = request.TagList.Where(item => item.Id.Value != 0).Select(item => item.Id).ToList();
                // 要刪除的標籤
                var deleteTagIdList = tagIdList.Except(needCheckTagIdList);
                // 新增的標籤
                var newTagList = request.TagList.Where(item => item.Id.Value == 0).ToList();
                var newTagIdList = new List<LstId>();
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
                #region 更新影片資訊和調整標籤關聯
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
                _logger.LogError(ex.ToString());
                throw;
            }
        }
    }
}
