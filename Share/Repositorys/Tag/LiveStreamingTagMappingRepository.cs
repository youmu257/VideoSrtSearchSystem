using MySqlConnector;
using Share.Models.LiveStraming;
using Share.Tool.MySQL;
using SqlKata;

namespace Share.Repositorys.Tag
{
    public class LiveStreamingTagMappingRepository(
        IMySqlTool _mySqlTool
    ) : ILiveStreamingTagMappingRepository
    {
        public int Delete(MySqlConnection connection, MySqlTransaction trans, LsId videoId, LstId tagId)
        {
            try
            {
                var query = new Query(LiveStreamingTagMappingModel.TableName)
                    .Where(nameof(LiveStreamingTagMappingModel.lstm_ls_id), "=", videoId.Value)
                    .Where(nameof(LiveStreamingTagMappingModel.lstm_lst_id), "=", tagId.Value)
                    .AsDelete();
                return _mySqlTool.Delete(connection, trans, query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Delete(MySqlConnection connection, MySqlTransaction trans, LsId videoId)
        {
            try
            {
                var query = new Query(LiveStreamingTagMappingModel.TableName)
                    .Where(nameof(LiveStreamingTagMappingModel.lstm_ls_id), "=", videoId.Value)
                    .AsDelete();
                return _mySqlTool.Delete(connection, trans, query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public uint Insert(MySqlConnection connection, MySqlTransaction trans, LsId videoId, List<LstId> tagIdList)
        {
            try
            {
                var insertCols = new string[]
                {
                    nameof(LiveStreamingTagMappingModel.lstm_ls_id),
                    nameof(LiveStreamingTagMappingModel.lstm_lst_id),
                };
                var insertData = tagIdList.Select(tagId => new List<object>
                {
                    videoId.Value,
                    tagId.Value,
                });
                var query = new Query(LiveStreamingTagMappingModel.TableName)
                    .AsInsert(insertCols, insertData);
                return _mySqlTool.Insert(connection, trans, query);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
