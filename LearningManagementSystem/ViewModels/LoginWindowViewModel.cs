using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LearningManagementSystem.ViewModels
{
    public class LoginWindowViewModel
    {
        public IDao _dao { get; set; }
        public User UserLogin { get; set; }

        public bool IsAdmin => UserLogin?.Role == "admin";

        public LoginWindowViewModel()
        {
            _dao = App.Current.Services.GetService<IDao>();;
            UserLogin = new User();
        }

        public bool CheckUserInfo()
        {
            return _dao.CheckUserInfo(UserLogin);
        }

        public void GetUser()
        {
            _dao.GetFullUser(UserLogin);
        }

        public bool IsExistsUsername()
        {
            return _dao.IsExistsUsername(UserLogin.Username);
        }

        public bool AddUser()
        {
            return _dao.AddUser(UserLogin);
        }
    }
}
