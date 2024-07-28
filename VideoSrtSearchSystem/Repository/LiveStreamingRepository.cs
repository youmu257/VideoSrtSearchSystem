using MySqlConnector;
using VideoSrtSearchSystem.Models.LiveStraming;
using VideoSrtSearchSystem.Tool.MySQL;

namespace VideoSrtSearchSystem.Repository
{
    public class LiveStreamingRepository(IMySQLConnectionProvider _mySQLConnectionProvider) : ILiveStreamingRepository
    {
        public List<LiveStreamingModel> GetAll()
        {
            try
            {
                List<LiveStreamingModel> list = new List<LiveStreamingModel>();
                using (MySqlConnection connection = _mySQLConnectionProvider.GetNormalCotext())
                {
                    connection.Open();

                    string query = $"SELECT * FROM {LiveStreamingModel.TableName}";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new LiveStreamingModel(reader));
                        }
                    }
                }
                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
