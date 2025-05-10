using Share.DTO.Request;
using Share.DTO.Response.Video;

namespace Share.Services.Video
{
    public interface ILiveStreamingService
    {
        GetAllVideoResponse GetAllVideo(SearchRequest request);
        GetOneVideoInfoResponse GetOneVideoInfo(string guid);
    }
}
