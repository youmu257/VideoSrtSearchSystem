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
    }
}
