using MySqlConnector;

namespace VideoSrtSearchSystem.Tool.MySQL
{
    public interface IMySQLConnectionProvider
    {
        MySqlConnection GetNormalCotext();
    }
}
