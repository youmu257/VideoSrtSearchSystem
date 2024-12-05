using MySqlConnector;
using Share.Models.LiveStraming;
using Share.Tool.MySQL;
using SqlKata;
using System.Data;

namespace Share.Repositorys.Srt
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
                    model.lss_ls_id.Value,
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

        public int DeleteByVideoId(MySqlConnection connection, MySqlTransaction trans, LsId videoId)
        {
            try
            {
                var query = new Query(LiveStreamingSrtModel.TableName)
                    .Where(nameof(LiveStreamingSrtModel.lss_ls_id), "=", videoId.Value)
                    .AsDelete();
                return _mySqlTool.Delete(connection, trans, query);
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
                    nameof(LiveStreamingModel.ls_livetime),
                };
                var subQuery = new Query(LiveStreamingModel.TableName)
                    .WhereLike(nameof(LiveStreamingModel.ls_all_srt), $"%{keyword}%")
                    .OrderBy(nameof(LiveStreamingModel.ls_createtime))
                    .Offset((page - 1) * pageSize)
                    .Limit(pageSize)
                    .Select(subcols);
                var cols = new string[]
                {
                    nameof(LiveStreamingModel.ls_title),
                    nameof(LiveStreamingModel.ls_guid),
                    nameof(LiveStreamingModel.ls_url),
                    nameof(LiveStreamingModel.ls_livetime),
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

        public List<LiveStreamingSrtModel> GetByLikeKeyword(List<LsId> lsIdList, string keyword, MySqlConnection connection)
        {
            try
            {
                var cols = new string[]
                {
                    nameof(LiveStreamingSrtModel.lss_ls_id),
                    nameof(LiveStreamingSrtModel.lss_text),
                    nameof(LiveStreamingSrtModel.lss_start),
                    nameof(LiveStreamingSrtModel.lss_end),
                };
                var query = new Query(LiveStreamingSrtModel.TableName)
                    .WhereIn(nameof(LiveStreamingSrtModel.lss_ls_id), lsIdList.Select(item => item.Value))
                    .WhereLike(nameof(LiveStreamingSrtModel.lss_text), $"%{keyword}%")
                    .Select(cols);
                return _mySqlTool.SelectMany<LiveStreamingSrtModel>(connection, query);
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
                var query = new Query(LiveStreamingModel.TableName)
                    .WhereLike(nameof(LiveStreamingModel.ls_all_srt), $"%{keyword}%")
                    .SelectRaw($"COUNT({nameof(LiveStreamingModel.ls_id)})");
                return _mySqlTool.Count(connection, query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<LiveStreamingSrtModel> GetByVideoId(LsId videoId, MySqlConnection connection)
        {
            try
            {
                var cols = new string[]
                {
                    nameof(LiveStreamingSrtModel.lss_num),
                    nameof(LiveStreamingSrtModel.lss_text),
                    nameof(LiveStreamingSrtModel.lss_start),
                    nameof(LiveStreamingSrtModel.lss_end),
                };
                var query = new Query(LiveStreamingSrtModel.TableName)
                    .Where(nameof(LiveStreamingSrtModel.lss_ls_id), "=", videoId.Value)
                    .OrderBy(nameof(LiveStreamingSrtModel.lss_num))
                    .Select(cols);
                return _mySqlTool.SelectMany<LiveStreamingSrtModel>(connection, query);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
