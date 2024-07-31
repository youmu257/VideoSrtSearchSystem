using MySqlConnector;
using VideoSrtSearchSystem.Models.LiveStraming;

namespace VideoSrtSearchSystem.Repository.Srt
{
    public interface ILiveStreamingSrtRepository
    {
        uint Insert(MySqlConnection connection, MySqlTransaction trans, List<LiveStreamingSrtModel> modelList);
    }
}
