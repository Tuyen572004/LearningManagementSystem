using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Services.CloudinaryService;
using System;
using System.IO;
using System.Text.Json;

namespace LearningManagementSystem.Services.ConfigService
{
    /// <summary>
    /// Service to handle configuration file operations.
    /// </summary>
    public class ConfigFileService : IConfigService
    {
        private readonly string _configFilePath;
        JsonDocument configJson;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigFileService"/> class.
        /// Loads the configuration file and parses its content.
        /// </summary>
        public ConfigFileService()
        {
            _configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            string configContent = File.ReadAllTextAsync(_configFilePath).Result;
            configJson = JsonDocument.Parse(configContent);
        }

        /// <summary>
        /// Gets the database configuration from the configuration file.
        /// </summary>
        /// <returns>A <see cref="DatabaseConfig"/> object containing the database configuration.</returns>
        public DatabaseConfig GetDatabaseConfig()
        {
            var databaseConfig = configJson.RootElement.GetProperty("PG_Database").ToString();
            return JsonSerializer.Deserialize<DatabaseConfig>(databaseConfig);
        }

        /// <summary>
        /// Gets the Cloudinary configuration from the configuration file.
        /// </summary>
        /// <returns>A <see cref="CloudinaryConfig"/> object containing the Cloudinary configuration.</returns>
        public CloudinaryConfig GetCloudinaryConfig()
        {
            var cloudinaryConfig = configJson.RootElement.GetProperty("Cloudinary").ToString();
            return JsonSerializer.Deserialize<CloudinaryConfig>(cloudinaryConfig);
        }

        /// <summary>
        /// Gets the Brevo email configuration from the configuration file.
        /// </summary>
        /// <returns>A <see cref="BrevoConfig"/> object containing the Brevo email configuration.</returns>
        public BrevoConfig GetBrevoConfig()
        {
            var brevoConfig = configJson.RootElement.GetProperty("BREVO_Email").ToString();
            return JsonSerializer.Deserialize<BrevoConfig>(brevoConfig);
        }
    }
}
