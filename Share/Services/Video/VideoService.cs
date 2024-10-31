using Microsoft.Extensions.Logging;
using Share.DTO.Request.Video;
using Share.DTO.Response.Video;
using Share.Repositorys.LiveStraming;
using Share.Tool;
using Share.Tool.MySQL;

namespace Share.Services.Video
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

        public GetOneVideoInfoResponse GetOneVideoInfo(string guid)
        {
            try
            {
                using var connection = _mySQLConnectionProvider.GetNormalCotext();
                // 取得影片資訊
                var liveStramingModel = _liveStreamingRepository.GetByGuid(guid, connection);
                return new GetOneVideoInfoResponse
                {
                    VideoGuid = guid,
                    VideoTitle = liveStramingModel.ls_title,
                    VideoUrl = liveStramingModel.ls_url,
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
