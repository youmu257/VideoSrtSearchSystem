using MySqlConnector;
using Share.Models.LiveStraming;

namespace Share.Repositorys.Tag
{
    public interface ILiveStreamingTagRepository
    {
        List<LiveStreamingTagModel> GetByTypeAndKeyword(LsttType type, string keyword, int page, int pageSize, MySqlConnection connection);
        List<UrlAndTagsModel> GetByTypeMapping(MySqlConnection connection);
        LiveStreamingTagModel GetById(LstId tagId, MySqlConnection connection);
        LiveStreamingTagModel GetByKeyword(string keyword, MySqlConnection connection);
        LiveStreamingTagModel GetByTypeAndKeyword(LsttType type, string keyword, MySqlConnection connection);
        List<LiveStreamingTagModel> GetByKeywordList(List<string> keywordList, MySqlConnection connection);
        int GetCount(LsttType type, string keyword, MySqlConnection? connection = null);
        List<LiveStreamingTagModel> GetByVideoGuid(string guid, MySqlConnection connection);
        uint Insert(MySqlConnection connection, MySqlTransaction trans, LiveStreamingTagModel model);
        int Update(MySqlConnection connection, MySqlTransaction trans, LiveStreamingTagModel model);
    }
}
