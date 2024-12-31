using LearningManagementSystem.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Services.UserService
{
    public class UserService
    {
        /// <summary>
        /// Gets the current user's role asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the role of the current user.</returns>
        public static async Task<string> GetCurrentUserRole()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var role = localSettings.Values["Role"] as string;
            return await Task.FromResult(role);
        }

        /// <summary>
        /// Saves the user configuration to local settings.
        /// </summary>
        /// <param name="user">The user object containing the configuration to save.</param>
        public static void SaveUserConfig(User user)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["Username"] = user.Username;
            localSettings.Values["Role"] = user.Role;
            localSettings.Values["Email"] = user.Email;
            localSettings.Values["Id"] = user.Id;
        }

        /// <summary>
        /// Encrypts the given password using Base64 encoding.
        /// </summary>
        /// <param name="password">The password to encrypt.</param>
        /// <returns>The encrypted password as a Base64 encoded string.</returns>
        public string EncryptPassword(string password)
        {
            var passwordRaw = password;
            var passwordInBytes = Encoding.UTF8.GetBytes(passwordRaw);

            var encryptedPasswordBase64 = Convert.ToBase64String(passwordInBytes);

            return encryptedPasswordBase64;
        }

        /// <summary>
        /// Logs out the current user by clearing the local settings.
        /// </summary>
        public void Logout()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["Username"] = null;
            localSettings.Values["Role"] = null;
            localSettings.Values["Email"] = null;
            localSettings.Values["Id"] = null;
        }

        /// <summary>
        /// Gets the current user asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the current user object, or null if no user is found.</returns>
        public static async Task<User> GetCurrentUser()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var id = localSettings.Values["Id"] as int?;
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
                    Id = (int)id,
                    CreatedAt = DateTime.Parse("1000-01-01 00:00:00"),
                    PasswordHash = "YWRtaW5zdHVkZW50",
                });
            }
            return null;
        }
    }
}
