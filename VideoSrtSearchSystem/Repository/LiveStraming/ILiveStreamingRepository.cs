using MySqlConnector;
using VideoSrtSearchSystem.Models.LiveStraming;

namespace VideoSrtSearchSystem.Repository.LiveStraming
{
    public interface ILiveStreamingRepository
    {
        LiveStreamingModel GetByUrl(string url, MySqlConnection? connection = null);
        uint Insert(MySqlConnection connection, MySqlTransaction trans, LiveStreamingModel model);
    }
}
