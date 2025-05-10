using MySqlConnector;
using Share.Models.LiveStraming;

namespace Share.Repositorys.Tag
{
    public interface ILiveStreamingTagRepository
    {
        List<LiveStreamingTagModel> GetByTypeAndKeyword(int type, string keyword, int page, int pageSize, MySqlConnection connection);
        int GetCount(int type, string keyword, MySqlConnection? connection = null);
    }
}
