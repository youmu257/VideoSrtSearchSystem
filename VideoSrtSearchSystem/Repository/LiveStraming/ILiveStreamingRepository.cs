using MySqlConnector;
using VideoSrtSearchSystem.Models.LiveStraming;

namespace VideoSrtSearchSystem.Repository.LiveStraming
{
    public interface ILiveStreamingRepository
    {
        List<LiveStreamingModel> GetAll(string keyword, int page, int pageSize, MySqlConnection? connection = null);
        LiveStreamingModel GetByUrl(string url, MySqlConnection? connection = null);
        uint Insert(MySqlConnection connection, MySqlTransaction trans, LiveStreamingModel model);
    }
}
