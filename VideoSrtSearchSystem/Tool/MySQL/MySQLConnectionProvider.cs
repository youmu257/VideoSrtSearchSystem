using MySqlConnector;

namespace VideoSrtSearchSystem.Tool.MySQL
{
    public class MySQLConnectionProvider(IConfiguration _configuration) : IMySQLConnectionProvider
    {
        public MySqlConnection GetNormalCotext()
        {
            return new MySqlConnection(_configuration["ConnectionStrings:localhost"]);
        }
    }
}
