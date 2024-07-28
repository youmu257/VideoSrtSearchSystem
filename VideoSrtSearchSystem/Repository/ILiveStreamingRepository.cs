using VideoSrtSearchSystem.Models.LiveStraming;

namespace VideoSrtSearchSystem.Repository
{
    public interface ILiveStreamingRepository
    {
        List<LiveStreamingModel> GetAll();
    }
}
