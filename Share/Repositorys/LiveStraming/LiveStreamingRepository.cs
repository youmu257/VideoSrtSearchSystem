﻿using MySqlConnector;
using Share.Models.LiveStraming;
using Share.Tool.MySQL;
using SqlKata;

namespace Share.Repositorys.LiveStraming
{
    public class LiveStreamingRepository(IMySqlTool _mySqlTool) : ILiveStreamingRepository
    {
        public List<LiveStreamingModel> GetAll(string keyword, int page, int pageSize, MySqlConnection? connection = null)
        {
            try
            {
                var cols = new string[]
                {
                    nameof(LiveStreamingModel.ls_guid),
                    nameof(LiveStreamingModel.ls_title),
                    nameof(LiveStreamingModel.ls_url),
                };
                var query = new Query(LiveStreamingModel.TableName);
                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.WhereLike(nameof(LiveStreamingModel.ls_title), $"%{keyword}%");
                }
                query = query.OrderByDesc(nameof(LiveStreamingModel.ls_createtime))
                    .Offset(page * pageSize)
                    .Limit(pageSize)
                    .Select(cols);
                return _mySqlTool.SelectMany<LiveStreamingModel>(connection, query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<LiveStreamingModel> GetAll(MySqlConnection? connection = null)
        {
            try
            {
                var cols = new string[]
                {
                    nameof(LiveStreamingModel.ls_id),
                    nameof(LiveStreamingModel.ls_guid),
                    nameof(LiveStreamingModel.ls_all_srt),
                    nameof(LiveStreamingModel.ls_title),
                    nameof(LiveStreamingModel.ls_url),
                    nameof(LiveStreamingModel.ls_livetime),
                };
                var query = new Query(LiveStreamingModel.TableName)
                    .Select(cols);
                return _mySqlTool.SelectMany<LiveStreamingModel>(connection, query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int GetCount(MySqlConnection? connection = null)
        {
            try
            {
                var cols = new string[]
                {
                    nameof(LiveStreamingModel.ls_id),
                };
                var query = new Query(LiveStreamingModel.TableName)
                    .AsCount(cols);
                return _mySqlTool.Count(connection, query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public LiveStreamingModel GetByUrl(string url, MySqlConnection? connection = null)
        {
            try
            {
                var cols = new string[]
                {
                    nameof(LiveStreamingModel.ls_id),
                    nameof(LiveStreamingModel.ls_guid),
                };
                var query = new Query(LiveStreamingModel.TableName)
                    .Where(nameof(LiveStreamingModel.ls_url), "=", url)
                    .Select(cols);
                return _mySqlTool.SelectOne<LiveStreamingModel>(connection, query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public LiveStreamingModel GetByGuid(string guid, MySqlConnection? connection = null)
        {
            try
            {
                var cols = new string[]
                {
                    nameof(LiveStreamingModel.ls_id),
                    nameof(LiveStreamingModel.ls_title),
                    nameof(LiveStreamingModel.ls_url),
                };
                var query = new Query(LiveStreamingModel.TableName)
                    .Where(nameof(LiveStreamingModel.ls_guid), "=", guid)
                    .Select(cols);
                return _mySqlTool.SelectOne<LiveStreamingModel>(connection, query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public LsId Insert(MySqlConnection connection, MySqlTransaction trans, LiveStreamingModel model)
        {
            try
            {
                var insertCols = new string[]
                {
                    nameof(LiveStreamingModel.ls_guid),
                    nameof(LiveStreamingModel.ls_title),
                    nameof(LiveStreamingModel.ls_url),
                    nameof(LiveStreamingModel.ls_livetime),
                    nameof(LiveStreamingModel.ls_all_srt),
                };
                var insertDataList = new List<object>
                {
                    model.ls_guid,
                    model.ls_title,
                    model.ls_url,
                    model.ls_livetime,
                    model.ls_all_srt,
                };
                var query = new Query(LiveStreamingModel.TableName)
                    .AsInsert(insertCols, insertDataList);
                return LsId.From(_mySqlTool.Insert(connection, trans, query));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int UpdateAllSrt(MySqlConnection connection, MySqlTransaction trans, string videoGuid, string allSrt)
        {
            try
            {
                var update = new Dictionary<string, object>()
                {
                    { nameof(LiveStreamingModel.ls_all_srt), allSrt },
                };
                var query = new Query(LiveStreamingModel.TableName)
                    .Where(nameof(LiveStreamingModel.ls_guid), videoGuid)
                    .AsUpdate(update);
                return _mySqlTool.Update(connection, trans, query);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
