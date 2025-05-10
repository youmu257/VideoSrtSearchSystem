using MySqlConnector;
using Share.Models.LiveStraming;
using Share.Tool.MySQL;
using SqlKata;

namespace Share.Repositorys.Tag
{
    public class LiveStreamingTagTypeRepository(
        IMySqlTool _mySqlTool
    ) : ILiveStreamingTagTypeRepository
    {
        public List<LiveStreamingTagTypeModel> GetAll(MySqlConnection connection)
        {
            try
            {
                var cols = new string[]
                {
                    nameof(LiveStreamingTagTypeModel.lstt_type),
                    nameof(LiveStreamingTagTypeModel.lstt_name),
                };
                var query = new Query(LiveStreamingTagTypeModel.TableName)
                    .Select(cols);
                return _mySqlTool.SelectMany<LiveStreamingTagTypeModel>(connection, query);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
