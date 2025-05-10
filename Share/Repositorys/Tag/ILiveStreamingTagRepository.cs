using MySqlConnector;
using Share.Models.LiveStraming;

namespace Share.Repositorys.Tag
{
    public interface ILiveStreamingTagRepository
    {
        List<LiveStreamingTagModel> GetByTypeAndKeyword(LsttType type, string keyword, int page, int pageSize, MySqlConnection connection);
        LiveStreamingTagModel GetByTypeAndKeyword(LsttType type, string keyword, MySqlConnection connection);
        int GetCount(LsttType type, string keyword, MySqlConnection? connection = null);
        uint Insert(MySqlConnection connection, MySqlTransaction trans, LiveStreamingTagModel model);
    }
}
