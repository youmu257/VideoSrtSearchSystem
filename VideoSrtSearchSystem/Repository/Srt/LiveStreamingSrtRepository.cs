using MySqlConnector;
using SqlKata;
using System.Data;
using VideoSrtSearchSystem.Models.LiveStraming;
using VideoSrtSearchSystem.Tool.MySQL;

namespace VideoSrtSearchSystem.Repository.Srt
{
    public class LiveStreamingSrtRepository(
        IMySqlTool _mySqlTool
    ) : ILiveStreamingSrtRepository
    {
        public uint Insert(MySqlConnection connection, MySqlTransaction trans, List<LiveStreamingSrtModel> modelList)
        {
            try
            {
                var insertCols = new string[]
                {
                    nameof(LiveStreamingSrtModel.lss_ls_id),
                    nameof(LiveStreamingSrtModel.lss_num),
                    nameof(LiveStreamingSrtModel.lss_start),
                    nameof(LiveStreamingSrtModel.lss_end),
                    nameof(LiveStreamingSrtModel.lss_text),
                };
                var insertDataList = modelList.Select(model => new List<object>
                {
                    model.lss_ls_id,
                    model.lss_num,
                    model.lss_start,
                    model.lss_end,
                    model.lss_text,
                }).ToList();
                var query = new Query(LiveStreamingSrtModel.TableName)
                    .AsInsert(insertCols, insertDataList);
                return _mySqlTool.Insert(connection, trans, query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<TwoModelData<LiveStreamingModel, LiveStreamingSrtModel>> GetByLikeKeyword(string keyword, int page, int pageSize, MySqlConnection connection)
        {
            try
            {
                var subcols = new string[]
                {
                    nameof(LiveStreamingModel.ls_id),
                    nameof(LiveStreamingModel.ls_title),
                    nameof(LiveStreamingModel.ls_guid),
                    nameof(LiveStreamingModel.ls_url),
                };
                var subQuery = new Query(LiveStreamingSrtModel.TableName)
                    .Join(LiveStreamingModel.TableName, nameof(LiveStreamingModel.ls_id), nameof(LiveStreamingSrtModel.lss_ls_id))
                    .WhereLike(nameof(LiveStreamingSrtModel.lss_text), $"%{keyword}%")
                    .OrderBy(nameof(LiveStreamingModel.ls_createtime))
                    .Offset((page - 1) * pageSize)
                    .Limit(pageSize)
                    .Select(subcols)
                    .Distinct();
                var cols = new string[]
                {
                    nameof(LiveStreamingModel.ls_title),
                    nameof(LiveStreamingModel.ls_guid),
                    nameof(LiveStreamingModel.ls_url),
                    nameof(LiveStreamingSrtModel.lss_text),
                    nameof(LiveStreamingSrtModel.lss_start),
                    nameof(LiveStreamingSrtModel.lss_end),
                };
                var query = new Query(LiveStreamingSrtModel.TableName)
                    .Join(
                        subQuery.As("A"),
                        q => q.On(nameof(LiveStreamingModel.ls_id), nameof(LiveStreamingSrtModel.lss_ls_id))
                    )
                    .WhereLike(nameof(LiveStreamingSrtModel.lss_text), $"%{keyword}%")
                    .Select(cols);
                return _mySqlTool.SelectMany<LiveStreamingModel, LiveStreamingSrtModel>(connection, query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int GetTotalPageByLikeKeyword(string keyword, MySqlConnection connection)
        {
            try
            {
                var query = new Query(LiveStreamingSrtModel.TableName)
                    .Join(LiveStreamingModel.TableName, nameof(LiveStreamingModel.ls_id), nameof(LiveStreamingSrtModel.lss_ls_id))
                    .WhereLike(nameof(LiveStreamingSrtModel.lss_text), $"%{keyword}%")
                    .SelectRaw($"COUNT(DISTINCT({nameof(LiveStreamingModel.ls_id)}))");
                return _mySqlTool.Count(connection, query);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
