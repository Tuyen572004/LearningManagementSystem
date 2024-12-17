using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Services.CloudinaryService;

namespace LearningManagementSystem.Services.ConfigService;

public interface IConfigService
{
    public DatabaseConfig GetDatabaseConfig();
    public CloudinaryConfig GetCloudinaryConfig();
    public BrevoConfig GetBrevoConfig();
}
