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

        private User CurrentUser { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public ProfilePageViewModel()
        {
            _dao = App.Current.Services.GetService<IDao>();
            CurrentUser = new User();
            CurrentUser = _dao.GetUserByUserName(UserService.GetCurrentUser().Result.Username);
            UserName = CurrentUser.Username;
            Email = CurrentUser.Email;
        }
        public void UpdateProfile()
        {
            CurrentUser.Username = UserName;
            CurrentUser.Email = Email;
            _dao.UpdateUser(CurrentUser);
        }



    }
}
