using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    public class LoginWindowViewModel
    {
        public SqlDao _dao { get; set; }
        public User UserLogin { get; set; }

        public LoginWindowViewModel()
        {
            _dao = new SqlDao();
            UserLogin = new User();
        }

        public bool CheckUserInfo()
        {
            //return _dao.CheckUserInfo(UserLogin);
            return true;
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
