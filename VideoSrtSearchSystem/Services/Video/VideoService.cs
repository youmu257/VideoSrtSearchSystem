using VideoSrtSearchSystem.DTO.Request.Video;
using VideoSrtSearchSystem.DTO.Response.Video;
using VideoSrtSearchSystem.Repository.LiveStraming;
using VideoSrtSearchSystem.Tool;
using VideoSrtSearchSystem.Tool.MySQL;

namespace VideoSrtSearchSystem.Services.Video
{
    public class VideoService(
        ILiveStreamingRepository _liveStreamingRepository,
        IMySQLConnectionProvider _mySQLConnectionProvider,
        ICommonTool _commonTool,
        ILogger<VideoService> _logger
    ) : IVideoService
    {
        private static readonly int pageSize = 25;

        public GetAllVideoResponse GetAllVideo(GetAllVideoRequest request)
        {
            try
            {
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                // 取得影片列表
                var liveStramingList = _liveStreamingRepository.GetAll(request.Keyword, request.Page - 1, pageSize, connection);
                var totalCount = _liveStreamingRepository.GetCount(connection);
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
    }
}
