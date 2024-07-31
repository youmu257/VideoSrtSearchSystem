﻿using MySqlConnector;
using SqlKata;
using VideoSrtSearchSystem.Models.LiveStraming;
using VideoSrtSearchSystem.Tool.MySQL;

namespace VideoSrtSearchSystem.Repository.LiveStraming
{
    public class LiveStreamingRepository(
        IMySqlTool _mySqlTool
    ) : ILiveStreamingRepository
    {
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

        public uint Insert(MySqlConnection connection, MySqlTransaction trans, LiveStreamingModel model)
        {
            try
            {
                var insertCols = new string[]
                {
                    nameof(LiveStreamingModel.ls_guid),
                    nameof(LiveStreamingModel.ls_title),
                    nameof(LiveStreamingModel.ls_url),
                };
                var insertDataList = new List<object>
                {
                    model.ls_guid,
                    model.ls_title,
                    model.ls_url,
                };
                var query = new Query(LiveStreamingModel.TableName)
                    .AsInsert(insertCols, insertDataList);
                return _mySqlTool.Insert(connection, trans, query);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}