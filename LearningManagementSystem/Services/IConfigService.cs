using LearningManagementSystem.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Services
{
    public interface IConfigService
    {
        public DatabaseConfig GetDatabaseConfig();
        public CloudinaryConfig GetCloudinaryConfig();
    }
}
