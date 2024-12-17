using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Services.CloudinaryService;
using System;
using System.IO;
using System.Text.Json;

namespace LearningManagementSystem.Services.ConfigService
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

        public BrevoConfig GetBrevoConfig()
        {
            var brevoConfig = configJson.RootElement.GetProperty("BREVO_Email").ToString();
            return JsonSerializer.Deserialize<BrevoConfig>(brevoConfig);
        }
    }
}
