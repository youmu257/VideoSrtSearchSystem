using Share.DTO.Request.Video;
using Share.DTO.Response.Video;

namespace Share.Services.Video
{
    public interface IVideoService
    {
        GetAllVideoResponse GetAllVideo(GetAllVideoRequest request);
        GetOneVideoInfoResponse GetOneVideoInfo(string guid);
    }
}
