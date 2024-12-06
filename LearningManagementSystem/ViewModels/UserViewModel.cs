using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        public string Keyword { get; set; } = "";
        public bool NameAscending { get; set; } = false;
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; } = 0;
        public int TotalItems { get; set; } = 0;

        private IDao _dao = null;

        public User SelectedUser { get; set; }

        public FullObservableCollection<User> Users { get; set; }

        public UserViewModel()
        {
            _dao = new SqlDao();
            SelectedUser = new User();
            GetAllUsers();
        }

        public void GetAllUsers()
        {
            var (totalItems, users) = _dao.GetAllUsers(CurrentPage, PageSize, Keyword, NameAscending);
            if (totalItems >= 0)
            {
                Users = new FullObservableCollection<User>(users);
                TotalItems = totalItems;
                TotalPages = (TotalItems / PageSize) + ((TotalItems % PageSize == 0) ? 0 : 1);
            }
        }

        public int InsertUser(User user)
        {
            int count = _dao.InsertUser(user);

            if (count == 1)
            {
                Users.Add(user);
            }

            return count;
        }

        public int CountCourse()
        {
            return _dao.CountCourse();
        }
        public void Load(int page)
        {
            CurrentPage = page;
            GetAllUsers();
        }

        public void RemoveUser(User user)
        {
            _dao.RemoveUserByID(user.Id);

        }

        public void UpdateUser(User user)
        {
            _dao.UpdateUser(user);
        }

    };

}
