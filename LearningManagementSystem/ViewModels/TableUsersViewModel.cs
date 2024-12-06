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
    public class TableUsersView : BaseViewModel, ICloneable
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public object Clone()
        {
            return new TableUsersView
            {
                ID = this.ID,
                Username = this.Username,
                PasswordHash = this.PasswordHash,
                Email = this.Email,
                Role = this.Role,
                CreatedAt = this.CreatedAt

            };
        }
    }
    public class TableUsersViewModel : BaseViewModel
    {
        private IDao _dao = null;

        public TableUsersView SelectedUser { get; set; }
        public FullObservableCollection<TableUsersView> TableUsers { get; set; }

        public string Keyword { get; set; } = "";
        public bool NameAcending { get; set; } = false;
        public int CurrentPage { get; set; } = 1;
        public int RowsPerPage { get; set; } = 10;

        public int TotalPages { get; set; } = 0;
        public int TotalItems { get; set; } = 0;

        public string[] TypeSort { get; set; } = { "Course Code", "Course Description" };
        public TableUsersViewModel()
        {
            TableUsers = new FullObservableCollection<TableUsersView>();
            _dao = new SqlDao();
            SelectedUser = new TableUsersView();

        }
        public void GetAllUser(int page = 1, int pageSize = 10, string keyword = "", bool nameAscending = false)
        {
            var (totalItems, users) = _dao.GetAllUsers(page, pageSize, keyword, nameAscending);
            if (users != null)
            {
                TableUsers.Clear();
                for (int i = 0; i < users.Count; i++)
                {
                    TableUsers.Add(new TableUsersView
                    {
                        ID = users[i].Id,
                        Username = users[i].Username,
                        PasswordHash = users[i].PasswordHash,
                        Email = users[i].Email,
                        Role = users[i].Role,
                        CreatedAt = users[i].CreatedAt,
                    });
                }
                TotalItems = totalItems;
                TotalPages = (TotalItems / RowsPerPage)
                    + ((TotalItems % RowsPerPage == 0)
                            ? 0 : 1);
            }
        }

        public int InsertUser(User user)
        {
            int count = _dao.InsertUser(user);

            if (count == 1)
            {
                //Courses.Add(course);
            }

            return count;
        }

        public void Load(int page)
        {
            CurrentPage = page;
            GetAllUser(CurrentPage, RowsPerPage);
        }

        public void RemoveUser(User user)
        {
            _dao.RemoveUserByID(user.Id);

        }
    }
}
