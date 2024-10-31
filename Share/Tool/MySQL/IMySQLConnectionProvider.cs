using MySqlConnector;

namespace Share.Tool.MySQL
{
    public interface IMySQLConnectionProvider
    {
        MySqlConnection GetNormalCotext();
    }
}
