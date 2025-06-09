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
                .OrderBy(nameof(LiveStreamingTagModel.lst_name))
                .Select(cols);
            return _mySqlTool.SelectMany<LiveStreamingTagModel>(connection, query);
        }

        public List<UrlAndTagsModel> GetByTypeMapping(MySqlConnection connection)
        {
            var cols = new string[]
            {
                nameof(UrlAndTagsModel.ls_url),
                nameof(LiveStreamingModel.ls_id),
            };
            var query = new Query(LiveStreamingModel.TableName)
                .Join(
                    LiveStreamingTagMappingModel.TableName,
                    nameof(LiveStreamingModel.ls_id),
                    nameof(LiveStreamingTagMappingModel.lstm_ls_id)
                )
                .Join(
                    LiveStreamingTagModel.TableName,
                    nameof(LiveStreamingTagMappingModel.lstm_lst_id),
                    nameof(LiveStreamingTagModel.lst_id)
                )
                .Select(cols)
                .SelectRaw(
                    @$"JSON_ARRAYAGG(JSON_OBJECT('name', {nameof(LiveStreamingTagModel.lst_name)}, 'type', {nameof(LiveStreamingTagModel.lst_type)})) AS {nameof(UrlAndTagsModel.tags)}"
                )
                .GroupBy(nameof(LiveStreamingModel.ls_url), nameof(LiveStreamingModel.ls_id))
                .OrderBy(nameof(LiveStreamingModel.ls_id));

            return _mySqlTool.SelectMany<UrlAndTagsModel>(connection, query);
        }

        public List<LiveStreamingTagModel> GetByType(int type, MySqlConnection connection)
        {
            var cols = new string[]
            {
                nameof(LiveStreamingTagModel.lst_name),
            };
            var query = new Query(LiveStreamingTagModel.TableName)
                .Where(nameof(LiveStreamingTagModel.lst_type), type)
                .Select(cols)
                .OrderBy(nameof(LiveStreamingTagModel.lst_name));
            return _mySqlTool.SelectMany<LiveStreamingTagModel>(connection, query);
        }

        public LiveStreamingTagModel GetById(LstId tagId, MySqlConnection connection)
        {
            var cols = new string[]
            {
                nameof(LiveStreamingTagModel.lst_type),
                nameof(LiveStreamingTagModel.lst_name),
            };
            var query = new Query(LiveStreamingTagModel.TableName)
                .Where(nameof(LiveStreamingTagModel.lst_id), tagId.Value)
                .Select(cols);
            return _mySqlTool.SelectOne<LiveStreamingTagModel>(connection, query);
        }

        public LiveStreamingTagModel GetByKeyword(string keyword, MySqlConnection connection)
        {
            var cols = new string[]
            {
                nameof(LiveStreamingTagModel.lst_id),
            };
            var query = new Query(LiveStreamingTagModel.TableName)
                .Where(nameof(LiveStreamingTagModel.lst_name), keyword)
                .Select(cols);
            return _mySqlTool.SelectOne<LiveStreamingTagModel>(connection, query);
        }

        public LiveStreamingTagModel GetByTypeAndKeyword(LsttType type, string keyword, MySqlConnection connection)
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

        public List<LiveStreamingTagModel> GetByKeywordList(List<string> keywordList, MySqlConnection connection)
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

        public int GetCount(LsttType type, string keyword, MySqlConnection? connection = null)
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

        public List<LiveStreamingTagModel> GetByVideoGuid(string guid, MySqlConnection connection)
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

        public uint Insert(MySqlConnection connection, MySqlTransaction trans, LiveStreamingTagModel model)
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

        public int Update(MySqlConnection connection, MySqlTransaction trans, LiveStreamingTagModel model)
        {
            var update = new Dictionary<string, object>()
            {
                { nameof(LiveStreamingTagModel.lst_name), model.lst_name },
                { nameof(LiveStreamingTagModel.lst_type), model.lst_type.Value },
            };
            var query = new Query(LiveStreamingTagModel.TableName)
                .Where(nameof(LiveStreamingTagModel.lst_id), model.lst_id.Value)
                .AsUpdate(update);
            return _mySqlTool.Update(connection, trans, query);
        }
    }
}
