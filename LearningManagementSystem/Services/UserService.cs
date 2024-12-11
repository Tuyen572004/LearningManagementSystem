using LearningManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Services
{
    public class UserService
    {
        //// TODO : Implement the GetUserRoleAsync method
        public static async Task<string> GetCurrentUserRole()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var role = localSettings.Values["Role"] as string;
            return await Task.FromResult(role);
        }

        public static void SaveUserConfig(User user)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["Username"] = user.Username;
            localSettings.Values["Role"] = user.Role;
            localSettings.Values["Email"] = user.Email;
        }

        public string EncryptPassword(string password)
        {
            var passwordRaw = password;
            var passwordInBytes = Encoding.UTF8.GetBytes(passwordRaw);

            var encryptedPasswordBase64 = Convert.ToBase64String(passwordInBytes);

            return encryptedPasswordBase64;
        }
        public static async Task<User> GetCurrentUser()
        {

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var username = localSettings.Values["Username"] as string;
            var role = localSettings.Values["Role"] as string;
            var email = localSettings.Values["Email"] as string;

            if (username != null && role != null && email != null)
            {
                return await Task.FromResult(new User
                {
                    Username = username,
                    Role = role,
                    Email = email,
                    Id = 1,
                    CreatedAt = DateTime.Parse("1000-01-01 00:00:00"),
                    PasswordHash = "YWRtaW5zdHVkZW50",
                });
            }
            return null;

        }
    }
}
