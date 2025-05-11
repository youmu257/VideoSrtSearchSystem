using Share.DTO.Request;
using Share.DTO.Request.Video;
using Share.DTO.Response.Video;

namespace Share.Services.Video
{
    public interface ILiveStreamingService
    {
        GetAllVideoResponse GetAllVideo(SearchRequest request);
        GetOneVideoInfoResponse GetOneVideoInfo(string guid);
        void UpdateVideoInfo(EditVideoRequest request);
    }
}
