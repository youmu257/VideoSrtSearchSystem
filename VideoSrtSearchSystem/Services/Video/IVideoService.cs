using VideoSrtSearchSystem.DTO.Request.Video;
using VideoSrtSearchSystem.DTO.Response.Video;

namespace VideoSrtSearchSystem.Services.Video
{
    public interface IVideoService
    {
        List<GetAllVideoResponse> GetAllVideo(GetAllVideoRequest request);
    }
}
