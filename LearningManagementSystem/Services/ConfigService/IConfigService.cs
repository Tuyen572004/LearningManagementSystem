using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Services.CloudinaryService;

namespace LearningManagementSystem.Services.ConfigService;

/// <summary>
/// Interface for configuration service to retrieve various configurations.
/// </summary>
public interface IConfigService
{
    /// <summary>
    /// Gets the database configuration.
    /// </summary>
    /// <returns>A <see cref="DatabaseConfig"/> object containing database configuration settings.</returns>
    public DatabaseConfig GetDatabaseConfig();

    /// <summary>
    /// Gets the Cloudinary configuration.
    /// </summary>
    /// <returns>A <see cref="CloudinaryConfig"/> object containing Cloudinary configuration settings.</returns>
    public CloudinaryConfig GetCloudinaryConfig();

    /// <summary>
    /// Gets the Brevo configuration.
    /// </summary>
    /// <returns>A <see cref="BrevoConfig"/> object containing Brevo configuration settings.</returns>
    public BrevoConfig GetBrevoConfig();
}
