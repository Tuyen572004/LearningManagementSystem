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
    public class ProfilePageViewModel : BaseViewModel
    {
        private IDao _dao = null;

        public UserService MyUserService { get; set; }
        private User CurrentUser { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
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
        public void UpdateProfile()
        {
            CurrentUser.Username = UserName;
            CurrentUser.Email = Email;
            CurrentUser.PasswordHash = Password;
            _dao.UpdateUser(CurrentUser);
        }
    }
}
