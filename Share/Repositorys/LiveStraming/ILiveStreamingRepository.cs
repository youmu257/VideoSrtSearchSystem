using MySqlConnector;
using Share.Models.LiveStraming;

namespace Share.Repositorys.LiveStraming
{
    public interface ILiveStreamingRepository
    {
        List<LiveStreamingModel> GetAll(string keyword, int page, int pageSize, MySqlConnection? connection = null);
        int GetCount(MySqlConnection? connection = null);
        LiveStreamingModel GetByUrl(string url, MySqlConnection? connection = null);
        LiveStreamingModel GetByGuid(string guid, MySqlConnection? connection = null);
        LsId Insert(MySqlConnection connection, MySqlTransaction trans, LiveStreamingModel model);
    }
}
