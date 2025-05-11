using MySqlConnector;
using Share.Models.LiveStraming;

namespace Share.Repositorys.Tag
{
    public interface ILiveStreamingTagMappingRepository
    {
        int Delete(MySqlConnection connection, MySqlTransaction trans, LsId videoId, LstId tagId);
        uint Insert(MySqlConnection connection, MySqlTransaction trans, LsId videoId, List<LstId> tagIdList);
    }
}
