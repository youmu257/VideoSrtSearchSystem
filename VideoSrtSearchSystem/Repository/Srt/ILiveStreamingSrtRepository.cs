using MySqlConnector;
using VideoSrtSearchSystem.Models.LiveStraming;
using VideoSrtSearchSystem.Tool.MySQL;

namespace VideoSrtSearchSystem.Repository.Srt
{
    public interface ILiveStreamingSrtRepository
    {
        uint Insert(MySqlConnection connection, MySqlTransaction trans, List<LiveStreamingSrtModel> modelList);
        int DeleteByVideoId(MySqlConnection connection, MySqlTransaction trans, uint videoId);
        List<TwoModelData<LiveStreamingModel, LiveStreamingSrtModel>> GetByLikeKeyword(string keyword, int page, int pageSize, MySqlConnection connection);
        int GetTotalPageByLikeKeyword(string keyword, MySqlConnection connection);
        List<LiveStreamingSrtModel> GetByVideoId(uint videoId, MySqlConnection connection);
    }
}
