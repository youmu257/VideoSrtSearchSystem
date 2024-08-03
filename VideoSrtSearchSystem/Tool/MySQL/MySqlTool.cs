using MySqlConnector;
using SqlKata;
using SqlKata.Compilers;
using System.Data;

namespace VideoSrtSearchSystem.Tool.MySQL
{
    public class MySqlTool(IMySQLConnectionProvider _mySQLConnectionProvider) : IMySqlTool
    {
        public void AddParametersToCommand(MySqlCommand command, Dictionary<string, object>? parameters = null)
        {
            if (parameters == null)
            {
                return;
            }

            foreach (KeyValuePair<string, object> parameter in parameters)
            {
                command.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }
        }

        public T SelectOne<T>(MySqlConnection? connection, Query query) where T : BaseModel, new()
        {
            var compiler = new MySqlCompiler();
            var compiledQuery = compiler.Compile(query);
            return SelectOne<T>(connection, compiledQuery.Sql, compiledQuery.NamedBindings);
        }

        public T SelectOne<T>(MySqlConnection? connection, string query, Dictionary<string, object>? parameters = null) where T : BaseModel, new()
        {
            bool nullConnection = connection == null;
            if (nullConnection)
            {
                connection = _mySQLConnectionProvider.GetNormalCotext();
            }
            connection!.Open();
            T model = new();
            using (var command = new MySqlCommand(query, connection))
            {
                if (parameters is not null)
                {
                    foreach (var item in parameters)
                    {
                        command.Parameters.AddWithValue(item.Key, item.Value);
                    }
                }
                using (var dr = command.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        model.Set(dr);
                    }

                }
            }
            if (nullConnection)
            {
                connection.Close();
            }
            return model;
        }

        public List<T> SelectMany<T>(MySqlConnection? connection, Query query) where T : BaseModel, new()
        {
            var compiler = new MySqlCompiler();
            var compiledQuery = compiler.Compile(query);
            return SelectMany<T>(connection, compiledQuery.Sql, compiledQuery.NamedBindings);
        }

        public List<T> SelectMany<T>(MySqlConnection? connection, string query, Dictionary<string, object>? parameters = null) where T : BaseModel, new()
        {
            bool nullConnection = connection == null;
            if (nullConnection)
            {
                connection = _mySQLConnectionProvider.GetNormalCotext();
            }
            connection!.Open();
            List<T> list = new();
            using (var command = new MySqlCommand(query, connection))
            {
                if (parameters is not null)
                {
                    foreach (var item in parameters)
                    {
                        command.Parameters.AddWithValue(item.Key, item.Value);
                    }
                }
                using (var dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        T model = new T();
                        model.Set(dr);
                        list.Add(model);
                    }
                }
            }
            if (nullConnection)
            {
                connection.Close();
            }
            return list;
        }

        public uint Insert(MySqlConnection connection, MySqlTransaction trans, Query query)
        {
            SqlResult sqlResult = new MySqlCompiler().Compile(query);
            return Insert(connection, trans, sqlResult.Sql, sqlResult.NamedBindings);
        }

        public uint Insert(MySqlConnection connection, MySqlTransaction trans, string query, Dictionary<string, object>? parameters = null)
        {
            uint result = 0;
            using (MySqlCommand mySqlCommand = new MySqlCommand(query, connection, trans))
            {
                AddParametersToCommand(mySqlCommand, parameters);
                mySqlCommand.ExecuteNonQuery();
            }
            using (MySqlCommand mySqlCommand2 = new MySqlCommand("SELECT LAST_INSERT_ID();", connection, trans))
            {
                using IDataReader dataReader = mySqlCommand2.ExecuteReader();
                if (dataReader != null && dataReader.Read())
                {
                    result = (uint)dataReader.GetInt64(0);
                }
            }
            return result;
        }
    }
}
