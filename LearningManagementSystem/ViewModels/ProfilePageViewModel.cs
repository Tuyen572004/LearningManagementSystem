using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Models;
using LearningManagementSystem.Services.UserService;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    /// <summary>
    /// ViewModel for the Profile Page.
    /// </summary>
    public class ProfilePageViewModel : BaseViewModel
    {
        private IDao _dao = null;

        /// <summary>
        /// Gets or sets the UserService instance.
        /// </summary>
        public UserService MyUserService { get; set; }

        /// <summary>
        /// Gets or sets the current user.
        /// </summary>
        private User CurrentUser { get; set; }

        /// <summary>
        /// Gets or sets the username of the current user.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the email of the current user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password of the current user.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfilePageViewModel"/> class.
        /// </summary>
        public ProfilePageViewModel()
        {
            MyUserService = new UserService();
            _dao = App.Current.Services.GetService<IDao>();
            CurrentUser = new User();
            CurrentUser = _dao.GetUserByUserName(UserService.GetCurrentUser().Result.Username);
            UserName = CurrentUser.Username;
            Email = CurrentUser.Email;
            Password = CurrentUser.PasswordHash;
        }

        /// <summary>
        /// Updates the profile of the current user.
        /// </summary>
        public void UpdateProfile()
        {
            CurrentUser.Username = UserName;
            CurrentUser.Email = Email;
            CurrentUser.PasswordHash = Password;
            _dao.UpdateUser(CurrentUser);
        }
    }
}
