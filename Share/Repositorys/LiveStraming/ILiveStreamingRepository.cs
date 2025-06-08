using MySqlConnector;
using Share.Models.LiveStraming;

namespace Share.Repositorys.LiveStraming
{
    public interface ILiveStreamingRepository
    {
        List<LiveStreamingModel> GetAll(string keyword, string tagKeyword, int page, int pageSize, MySqlConnection? connection = null);
        List<LiveStreamingModel> GetAll(MySqlConnection? connection = null);
        int GetCount(string keyword, string tagKeyword, MySqlConnection? connection = null);
        LiveStreamingModel GetByUrl(string url, MySqlConnection? connection = null);
        LiveStreamingModel GetByGuid(string guid, MySqlConnection? connection = null);
        LsId Insert(MySqlConnection connection, MySqlTransaction trans, LiveStreamingModel model);
        int UpdateAllSrt(MySqlConnection connection, MySqlTransaction trans, string videoGuid, string url, string title, string allSrt);
        int UpdateTitle(MySqlConnection connection, MySqlTransaction trans, string videoGuid, string title);
    }
}
