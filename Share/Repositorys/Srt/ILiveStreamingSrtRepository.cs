using MySqlConnector;
using Share.Models.LiveStraming;
using Share.Tool.MySQL;

namespace Share.Repositorys.Srt
{
    public interface ILiveStreamingSrtRepository
    {
        uint Insert(MySqlConnection connection, MySqlTransaction trans, List<LiveStreamingSrtModel> modelList);
        int DeleteByVideoId(MySqlConnection connection, MySqlTransaction trans, LsId videoId);
        List<TwoModelData<LiveStreamingModel, LiveStreamingSrtModel>> GetByLikeKeyword(string keyword, int page, int pageSize, MySqlConnection connection);
        int GetTotalPageByLikeKeyword(string keyword, MySqlConnection connection);
        List<LiveStreamingSrtModel> GetByVideoId(LsId videoId, MySqlConnection connection);
    }
}
