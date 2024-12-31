using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LearningManagementSystem.ViewModels
{
    /// <summary>
    /// ViewModel for the Login Window.
    /// </summary>
    public class LoginWindowViewModel
    {
        /// <summary>
        /// Data Access Object for interacting with the database.
        /// </summary>
        public IDao _dao { get; set; }

        /// <summary>
        /// User object representing the user attempting to log in.
        /// </summary>
        public User UserLogin { get; set; }

        /// <summary>
        /// Gets a value indicating whether the logged-in user is an admin.
        /// </summary>
        public bool IsAdmin => UserLogin?.Role == "admin";

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginWindowViewModel"/> class.
        /// </summary>
        public LoginWindowViewModel()
        {
            _dao = App.Current.Services.GetService<IDao>();
            UserLogin = new User();
        }

        /// <summary>
        /// Checks if the user information is valid.
        /// </summary>
        /// <returns>True if the user information is valid, otherwise false.</returns>
        public bool CheckUserInfo()
        {
            return _dao.CheckUserInfo(UserLogin);
        }

        /// <summary>
        /// Retrieves the full user information from the database.
        /// </summary>
        public void GetUser()
        {
            _dao.GetFullUser(UserLogin);
        }

        /// <summary>
        /// Checks if the username already exists in the database.
        /// </summary>
        /// <returns>True if the username exists, otherwise false.</returns>
        public bool IsExistsUsername()
        {
            return _dao.IsExistsUsername(UserLogin.Username);
        }

        /// <summary>
        /// Adds a new user to the database.
        /// </summary>
        /// <returns>True if the user was added successfully, otherwise false.</returns>
        public bool AddUser()
        {
            return _dao.AddUser(UserLogin);
        }
    }
}
