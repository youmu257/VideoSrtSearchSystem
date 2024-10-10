﻿using MySqlConnector;
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
            if (connection!.State != ConnectionState.Open)
            {
                connection.Open();
            }
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

        public int Count(MySqlConnection? connection, Query query)
        {
            var compiler = new MySqlCompiler();
            var compiledQuery = compiler.Compile(query);
            var result = SelectOneRaw(connection, compiledQuery.Sql, compiledQuery.NamedBindings);
            return Convert.ToInt32(result.FirstOrDefault().Value);
        }

        public Dictionary<string, object> SelectOneRaw(MySqlConnection? connection, Query query)
        {
            var compiler = new MySqlCompiler();
            var compiledQuery = compiler.Compile(query);
            return SelectOneRaw(connection, compiledQuery.Sql, compiledQuery.NamedBindings);
        }

        public Dictionary<string, object> SelectOneRaw(MySqlConnection? connection, string query, Dictionary<string, object>? parameters = null)
        {
            if (connection == null)
            {
                connection = _mySQLConnectionProvider.GetNormalCotext();
            }
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            using (var command = new MySqlCommand(query, connection))
            {
                AddParametersToCommand(command, parameters);
                using (var dr = command.ExecuteReader())
                {
                    var model = new Dictionary<string, object>();
                    if (dr.Read())
                    {
                        for (var i = 0; i < dr.FieldCount; i++)
                        {
                            model.Add(dr.GetName(i), dr.GetValue(i));
                        }
                    }
                    return model;
                }
            }
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
            if (connection!.State != ConnectionState.Open)
            {
                connection.Open();
            }
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

        public List<TwoModelData<K, V>> SelectMany<K, V>(MySqlConnection? connection, Query query)
            where K : BaseModel, new()
            where V : BaseModel, new()
        {
            var compiler = new MySqlCompiler();
            var compiledQuery = compiler.Compile(query);
            return SelectMany<K, V>(connection, compiledQuery.Sql, compiledQuery.NamedBindings);
        }

        public List<TwoModelData<K, V>> SelectMany<K, V>(MySqlConnection? connection, string query, Dictionary<string, object>? parameters = null)
            where K : BaseModel, new()
            where V : BaseModel, new()
        {
            bool nullConnection = connection == null;
            if (nullConnection)
            {
                connection = _mySQLConnectionProvider.GetNormalCotext();
            }
            if (connection!.State != ConnectionState.Open)
            {
                connection.Open();
            }
            List<TwoModelData<K, V>> list = new();
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
                        K modelK = new K();
                        V modelV = new V();
                        modelK.Set(dr);
                        modelV.Set(dr);
                        list.Add(new TwoModelData<K, V>(modelK, modelV));
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

        public int Delete(MySqlConnection connection, MySqlTransaction trans, Query query)
        {
            SqlResult sqlResult = new MySqlCompiler().Compile(query);
            return Delete(connection, trans, sqlResult.Sql, sqlResult.NamedBindings);
        }

        public int Delete(MySqlConnection connection, MySqlTransaction trans, string query, Dictionary<string, object>? parameters = null)
        {
            int result = 0;
            using (MySqlCommand mySqlCommand = new MySqlCommand(query, connection, trans))
            {
                AddParametersToCommand(mySqlCommand, parameters);
                result = mySqlCommand.ExecuteNonQuery();
            }
            return result;
        }
    }
}
