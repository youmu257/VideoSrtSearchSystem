using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Share.Tool.MySQL
{
    public class MySQLConnectionProvider(IConfiguration _configuration) : IMySQLConnectionProvider
    {
        public MySqlConnection GetNormalCotext()
        {
            return new MySqlConnection(_configuration["ConnectionStrings:Default"]);
        }
    }
}
