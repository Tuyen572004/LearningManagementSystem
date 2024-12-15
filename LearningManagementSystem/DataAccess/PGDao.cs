using Npgsql;
using System;
using LearningManagementSystem.Services;
using System.Text.Json;
using System.IO;

namespace LearningManagementSystem.DataAccess
{
    public class DatabaseConfig
    {
        public string DB_HOST { get; set; }
        public string DB_DATABASE { get; set; }
        public string DB_USERNAME { get; set; }
        public string DB_PASSWORD { get; set; }
    }

    public partial class SqlDao : IDao
    {
            public NpgsqlConnection connection;

            public string host { get; set; }
            public string database { get; set; }
            public string username { get; set; }
            public string password { get; set; }

        public SqlDao(IConfigService configService)
        {
            DatabaseConfig databaseConfig = configService.GetDatabaseConfig();
            host = databaseConfig.DB_HOST;
            database = databaseConfig.DB_DATABASE;
            username = databaseConfig.DB_USERNAME;
            password = databaseConfig.DB_PASSWORD;
            string connectionString = $"Host={host};Username={username};Password={password};Database={database}";
            connection = new NpgsqlConnection(connectionString);
        }

        public SqlDao()
            {
                Initialize();
            }

            public void Initialize()
            {
                string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
                string configContent = File.ReadAllText(configFilePath);
                var configJson = JsonDocument.Parse(configContent);
                var databaseConfig = configJson.RootElement.GetProperty("PG_Database").ToString();
                DatabaseConfig config = JsonSerializer.Deserialize<DatabaseConfig>(databaseConfig);

                host = config.DB_HOST;
                database = config.DB_DATABASE;
                username = config.DB_USERNAME;
                password = config.DB_PASSWORD;

                string connectionString = $"Host={host};Username={username};Password={password};Database={database}";
                connection = new NpgsqlConnection(connectionString);
            }

            public bool OpenConnection()
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch (NpgsqlException ex)
                {
                    Console.WriteLine($"Cannot connect to server: {ex.Message}");
                    return false;
                }
            }

            public bool CloseConnection()
            {
                try
                {
                    connection.Close();
                    return true;
                }
                catch (NpgsqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }

            public string GetFullCommandText(NpgsqlCommand command)
        {
            string commandText = command.CommandText;
            foreach (NpgsqlParameter param in command.Parameters)
            {
                commandText = commandText.Replace(param.ParameterName, param.Value.ToString());
            }
            return commandText;
        }

        //public void Dispose()
        //{
        //    if (_connection != null)
        //    {
        //        _connection.Dispose();
        //    }
        //}
    }
}