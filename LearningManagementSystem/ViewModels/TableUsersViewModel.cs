using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

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
        public string SortBy { get; set; } = "Id";
        public string SortOrder { get; set; } = "ASC";
        public int CurrentPage { get; set; } = 1;
        public int RowsPerPage { get; set; } = 10;
        public List<string> Suggestion { get; set; }
        public int TotalPages { get; set; } = 0;
        public int TotalItems { get; set; } = 0;

        public int countRepeatButton { get; set; } = 0;
        public TableUsersViewModel()
        {
            TableUsers = new FullObservableCollection<TableUsersView>();
            _dao = App.Current.Services.GetService<IDao>(); ;
            SelectedUser = new TableUsersView();
            Suggestion = _dao.GetAllUsernames();
        }
        public void GetAllUser()
        {
            var (totalItems, users) = _dao.GetAllUsers(CurrentPage, RowsPerPage, Keyword, SortBy, SortOrder);
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

        public void Load(int page = 1)
        {
            CurrentPage = page;
            GetAllUser();
        }

        public int RemoveUser(User user)
        {
            return _dao.RemoveUserByID(user.Id);

        }

        public int SetNullUserID(User user)
        {
            if (user.Role == "teacher")
                return _dao.SetNullUserIDInTeacher(user);
            return _dao.SetNullUserIDInStudent(user);
        }
    }
}
