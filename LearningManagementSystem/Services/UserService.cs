using LearningManagementSystem.EModels;
using System;
using System.Threading.Tasks;

namespace LearningManagementSystem.Services
{
    public class UserService
    {
        // TODO : Implement the GetUserRoleAsync method
        public async Task<string> getCurrentUserRole()
        {
            return await Task.FromResult("Student");
        }

        public async Task<User> GetCurrentUser()
        {
            return await Task.FromResult(new User
            {
                Id = 1,
                Username = "S001",
                PasswordHash = "YWRtaW5zdHVkZW50",
                Email = "email",
                Role = "student",
                CreatedAt = DateTime.Parse("1000-01-01 00:00:00")
            });
        }
    }
}
