using MySqlConnector;
using Share.Models.LiveStraming;

namespace Share.Repositorys.Tag
{
    public interface ILiveStreamingTagTypeRepository
    {
        List<LiveStreamingTagTypeModel> GetAll(MySqlConnection connection);
    }
}
