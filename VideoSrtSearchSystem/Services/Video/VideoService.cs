using VideoSrtSearchSystem.DTO.Request.Video;
using VideoSrtSearchSystem.DTO.Response.Video;
using VideoSrtSearchSystem.Repository.LiveStraming;
using VideoSrtSearchSystem.Tool.MySQL;

namespace VideoSrtSearchSystem.Services.Video
{
    public class VideoService(
        ILiveStreamingRepository _liveStreamingRepository,
        IMySQLConnectionProvider _mySQLConnectionProvider,
        ILogger<VideoService> _logger
    ) : IVideoService
    {
        private static readonly int pageSize = 25;
        public List<GetAllVideoResponse> GetAllVideo(GetAllVideoRequest request)
        {
            try
            {
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                // 取得影片列表
                var liveStramingList = _liveStreamingRepository.GetAll(request.Keyword, request.Page - 1, pageSize, connection);
                return liveStramingList.Select(item => new GetAllVideoResponse
                {
                    VideoGuid = item.ls_guid,
                    VideoTitle = item.ls_title,
                    VideoUrl = item.ls_url,
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
    }
}
