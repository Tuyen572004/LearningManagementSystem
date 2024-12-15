using CommunityToolkit.WinUI.UI.Controls;
using LearningManagementSystem.Controls;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using LearningManagementSystem.ViewModels;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

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
                connection.Close();
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



    }
}