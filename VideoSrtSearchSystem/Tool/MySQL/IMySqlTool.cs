using MySqlConnector;
using SqlKata;

namespace VideoSrtSearchSystem.Tool.MySQL
{
    public interface IMySqlTool
    {
        void AddParametersToCommand(MySqlCommand command, Dictionary<string, object>? parameters = null);
        T SelectOne<T>(MySqlConnection? connection, Query query) where T : BaseModel, new();
        T SelectOne<T>(MySqlConnection? connection, string query, Dictionary<string, object>? parameters = null) where T : BaseModel, new();
        uint Insert(MySqlConnection connection, MySqlTransaction trans, Query query);
        uint Insert(MySqlConnection connection, MySqlTransaction trans, string query, Dictionary<string, object>? parameters = null);
    }
}
