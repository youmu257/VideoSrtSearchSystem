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
        public List<LiveStreamingTagModel> GetByTypeAndKeyword(int type, string keyword, int page, int pageSize, MySqlConnection connection)
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
                if (type > 0)
                {
                    query = query.Where(nameof(LiveStreamingTagModel.lst_type), type);
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

        public int GetCount(int type, string keyword, MySqlConnection? connection = null)
        {
            try
            {
                var cols = new string[]
                {
                    nameof(LiveStreamingTagModel.lst_id),
                };
                var query = new Query(LiveStreamingTagModel.TableName);
                if (type > 0)
                {
                    query = query.Where(nameof(LiveStreamingTagModel.lst_type), type);
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

    }
}
