using VideoSrtSearchSystem.DTO.Request.Video;
using VideoSrtSearchSystem.DTO.Response.Video;

namespace VideoSrtSearchSystem.Services.Video
{
    public interface IVideoService
    {
        GetAllVideoResponse GetAllVideo(GetAllVideoRequest request);
    }
}
