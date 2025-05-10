using Microsoft.Extensions.Logging;
using Share.DTO.Request.Tag;
using Share.DTO.Response.Tag;
using Share.Repositorys.Tag;
using Share.Services.Video;
using Share.Tool;
using Share.Tool.MySQL;

namespace Share.Services.Tag
{
    public class TagService(
        ILiveStreamingTagRepository _liveStreamingTagRepository,
        ILiveStreamingTagTypeRepository _liveStreamingTagTypeRepository,
        IMySQLConnectionProvider _mySQLConnectionProvider,
        ICommonTool _commonTool,
        ILogger<LiveStreamingService> _logger
    ) : ITagService
    {
        private const int pageSize = 25;

        public GetAllTagResponse SearchTags(SearchTagRequest request)
        {
            try
            {
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                // 取得標籤列表
                var tagList = _liveStreamingTagRepository.GetByTypeAndKeyword(request.Type, request.Keyword, request.Page - 1, pageSize, connection);
                var totalCount = _liveStreamingTagRepository.GetCount(request.Type, request.Keyword, connection);
                // 取得標邊類型列表
                var tagTypeList = _liveStreamingTagTypeRepository.GetAll(connection);
                var tagTypeDict = tagTypeList.ToDictionary(item => item.lstt_type, item => item.lstt_name);
                return new GetAllTagResponse
                {
                    TotalPage = _commonTool.GetTotalPage(totalCount, pageSize),
                    TagList = tagList.Select(item => new TagResponse
                    {
                        Id = item.lst_id,
                        TagName = item.lst_name,
                        TagType = tagTypeDict[item.lst_type],
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
    }
}
