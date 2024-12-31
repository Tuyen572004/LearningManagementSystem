using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace LearningManagementSystem.ViewModels
{
    /// <summary>
    /// Represents a view model for a user in the table.
    /// </summary>
    public class TableUsersView : BaseViewModel, ICloneable
    {
        /// <summary>
        /// Gets or sets the ID of the user.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password hash of the user.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets the email of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the role of the user.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the user.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
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
    /// <summary>
    /// ViewModel for managing the table of users.
    /// </summary>
    public class TableUsersViewModel : BaseViewModel
    {
        private IDao _dao = null;

        /// <summary>
        /// Gets or sets the selected user.
        /// </summary>
        public TableUsersView SelectedUser { get; set; }

        /// <summary>
        /// Gets or sets the collection of users in the table.
        /// </summary>
        public FullObservableCollection<TableUsersView> TableUsers { get; set; }

        /// <summary>
        /// Gets or sets the keyword for searching users.
        /// </summary>
        public string Keyword { get; set; } = "";

        /// <summary>
        /// Gets or sets the column to sort by.
        /// </summary>
        public string SortBy { get; set; } = "Id";

        /// <summary>
        /// Gets or sets the sort order (ASC or DESC).
        /// </summary>
        public string SortOrder { get; set; } = "ASC";

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        public int CurrentPage { get; set; } = 1;

        /// <summary>
        /// Gets or sets the number of rows per page.
        /// </summary>
        public int RowsPerPage { get; set; } = 10;

        /// <summary>
        /// Gets or sets the list of username suggestions.
        /// </summary>
        public List<string> Suggestion { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages.
        /// </summary>
        public int TotalPages { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total number of items.
        /// </summary>
        public int TotalItems { get; set; } = 0;

        /// <summary>
        /// Gets or sets the count of repeat button clicks.
        /// </summary>
        public int countRepeatButton { get; set; } = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableUsersViewModel"/> class.
        /// </summary>
        public TableUsersViewModel()
        {
            TableUsers = new FullObservableCollection<TableUsersView>();
            _dao = App.Current.Services.GetService<IDao>();
            SelectedUser = new TableUsersView();
            Suggestion = _dao.GetAllUsernames();
        }

        /// <summary>
        /// Retrieves all users based on the current filter and pagination settings.
        /// </summary>
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

        /// <summary>
        /// Inserts a new user into the database.
        /// </summary>
        /// <param name="user">The user to insert.</param>
        /// <returns>The number of rows affected.</returns>
        public int InsertUser(User user)
        {
            int count = _dao.InsertUser(user);

            if (count == 1)
            {
                //Courses.Add(course);
            }

            return count;
        }

        /// <summary>
        /// Loads the users for the specified page.
        /// </summary>
        /// <param name="page">The page number to load.</param>
        public void Load(int page = 1)
        {
            CurrentPage = page;
            GetAllUser();
        }

        /// <summary>
        /// Removes a user from the database.
        /// </summary>
        /// <param name="user">The user to remove.</param>
        /// <returns>The number of rows affected.</returns>
        public int RemoveUser(User user)
        {
            return _dao.RemoveUserByID(user.Id);
        }

        /// <summary>
        /// Sets the user ID to null for the specified user.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <returns>The number of rows affected.</returns>
        public int SetNullUserID(User user)
        {
            if (user.Role == "teacher")
                return _dao.SetNullUserIDInTeacher(user);
            return _dao.SetNullUserIDInStudent(user);
        }
    }
}
