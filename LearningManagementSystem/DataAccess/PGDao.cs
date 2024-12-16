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
        private readonly string _connectionString;

        public SqlDao(IConfigService configService)
        {
            var config = configService.GetDatabaseConfig();
            _connectionString = $"Host={config.DB_HOST};Username={config.DB_USERNAME};Password={config.DB_PASSWORD};Database={config.DB_DATABASE};Pooling=true;Minimum Pool Size=1;Maximum Pool Size=20;";
        }

        private NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
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
    }
}