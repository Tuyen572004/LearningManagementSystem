using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using LearningManagementSystem.DataAccess;
using NPOI.SS.Formula.Functions;

namespace LearningManagementSystem.Services
{
    public class ConfigFileService : IConfigService
    {
        private readonly string _configFilePath;
        JsonDocument configJson;

        public ConfigFileService()
        {
            _configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            string configContent = File.ReadAllTextAsync(_configFilePath).Result;
            configJson = JsonDocument.Parse(configContent);
        }

        public DatabaseConfig GetDatabaseConfig()
        {
            var databaseConfig = configJson.RootElement.GetProperty("PG_Database").ToString();
            return JsonSerializer.Deserialize<DatabaseConfig>(databaseConfig);
        }

        public CloudinaryConfig GetCloudinaryConfig()
        {
            var cloudinaryConfig = configJson.RootElement.GetProperty("Cloudinary").ToString();
            return JsonSerializer.Deserialize<CloudinaryConfig>(cloudinaryConfig);
        }
    }
}
