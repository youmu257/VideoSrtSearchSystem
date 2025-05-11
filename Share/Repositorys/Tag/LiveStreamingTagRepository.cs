using MySqlConnector;
using Share.Models.LiveStraming;
using Share.Tool.MySQL;
using SqlKata;

namespace Share.Repositorys.Tag
{
    public class LiveStreamingTagRepository(
        IMySqlTool _mySqlTool
    ) : ILiveStreamingTagRepository
    {
        public List<LiveStreamingTagModel> GetByTypeAndKeyword(LsttType type, string keyword, int page, int pageSize, MySqlConnection connection)
        {
            try
            {
                var cols = new string[]
                {
                    nameof(LiveStreamingTagModel.lst_id),
                    nameof(LiveStreamingTagModel.lst_name),
                    nameof(LiveStreamingTagModel.lst_type),
                };
                var query = new Query(LiveStreamingTagModel.TableName);
                if (type.Value > 0)
                {
                    query = query.Where(nameof(LiveStreamingTagModel.lst_type), type.Value);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.WhereLike(nameof(LiveStreamingTagModel.lst_name), $"%{keyword}%");
                }
                query = query
                    .Offset(page * pageSize)
                    .Limit(pageSize)
                    .Select(cols);
                return _mySqlTool.SelectMany<LiveStreamingTagModel>(connection, query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public LiveStreamingTagModel GetByTypeAndKeyword(LsttType type, string keyword, MySqlConnection connection)
        {
            try
            {
                var cols = new string[]
                {
                    nameof(LiveStreamingTagModel.lst_id),
                };
                var query = new Query(LiveStreamingTagModel.TableName)
                    .Where(nameof(LiveStreamingTagModel.lst_type), type.Value)
                    .WhereLike(nameof(LiveStreamingTagModel.lst_name), $"%{keyword}%")
                    .Select(cols);
                return _mySqlTool.SelectOne<LiveStreamingTagModel>(connection, query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<LiveStreamingTagModel> GetByKeywordList(List<string> keywordList, MySqlConnection connection)
        {
            try
            {
                var cols = new string[]
                {
                    nameof(LiveStreamingTagModel.lst_id),
                };
                var query = new Query(LiveStreamingTagModel.TableName)
                    .WhereIn(nameof(LiveStreamingTagModel.lst_name), keywordList)
                    .Select(cols);
                return _mySqlTool.SelectMany<LiveStreamingTagModel>(connection, query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int GetCount(LsttType type, string keyword, MySqlConnection? connection = null)
        {
            try
            {
                var cols = new string[]
                {
                    nameof(LiveStreamingTagModel.lst_id),
                };
                var query = new Query(LiveStreamingTagModel.TableName);
                if (type.Value > 0)
                {
                    query = query.Where(nameof(LiveStreamingTagModel.lst_type), type.Value);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.WhereLike(nameof(LiveStreamingTagModel.lst_name), $"%{keyword}%");
                }
                query = query.AsCount(cols);
                return _mySqlTool.Count(connection, query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<LiveStreamingTagModel> GetByVideoGuid(string guid, MySqlConnection connection)
        {
            try
            {
                var cols = new string[]
                {
                    nameof(LiveStreamingTagModel.lst_id),
                    nameof(LiveStreamingTagModel.lst_name),
                    nameof(LiveStreamingTagModel.lst_type),
                };
                var query = new Query(LiveStreamingModel.TableName)
                    .Join(LiveStreamingTagMappingModel.TableName, nameof(LiveStreamingModel.ls_id), nameof(LiveStreamingTagMappingModel.lstm_ls_id))
                    .Join(LiveStreamingTagModel.TableName, nameof(LiveStreamingTagModel.lst_id), nameof(LiveStreamingTagMappingModel.lstm_lst_id))
                    .Where(nameof(LiveStreamingModel.ls_guid), guid)
                    .Select(cols);
                return _mySqlTool.SelectMany<LiveStreamingTagModel>(connection, query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public uint Insert(MySqlConnection connection, MySqlTransaction trans, LiveStreamingTagModel model)
        {
            try
            {
                var insertCols = new string[]
                {
                    nameof(LiveStreamingTagModel.lst_name),
                    nameof(LiveStreamingTagModel.lst_type),
                };
                var insertData = new List<object>
                {
                    model.lst_name,
                    model.lst_type.Value,
                };
                var query = new Query(LiveStreamingTagModel.TableName)
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
