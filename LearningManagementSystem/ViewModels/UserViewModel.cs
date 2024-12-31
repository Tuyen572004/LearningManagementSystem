using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LearningManagementSystem.ViewModels
{
    /// <summary>
    /// ViewModel for managing user-related operations and data.
    /// </summary>
    public class UserViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the keyword for filtering users.
        /// </summary>
        public string Keyword { get; set; } = "";

        /// <summary>
        /// Gets or sets the field by which to sort users.
        /// </summary>
        public string SortBy { get; set; } = "Id";

        /// <summary>
        /// Gets or sets the sort order (ASC or DESC).
        /// </summary>
        public string SortOrder { get; set; } = "ASC";

        /// <summary>
        /// Gets or sets the current page number for pagination.
        /// </summary>
        public int CurrentPage { get; set; } = 1;

        /// <summary>
        /// Gets or sets the number of rows per page for pagination.
        /// </summary>
        public int RowsPerPage { get; set; } = 10;

        /// <summary>
        /// Gets or sets the total number of pages based on the total items and rows per page.
        /// </summary>
        public int TotalPages { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total number of items.
        /// </summary>
        public int TotalItems { get; set; } = 0;

        private IDao _dao = null;

        /// <summary>
        /// Gets or sets the selected user.
        /// </summary>
        public User SelectedUser { get; set; }

        /// <summary>
        /// Gets or sets the collection of users.
        /// </summary>
        public FullObservableCollection<User> Users { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserViewModel"/> class.
        /// </summary>
        public UserViewModel()
        {
            _dao = App.Current.Services.GetService<IDao>(); ;
            SelectedUser = new User();
            GetAllUsers();
        }

        /// <summary>
        /// Retrieves all users based on the current filter, sort, and pagination settings.
        /// </summary>
        public void GetAllUsers()
        {
            var (totalItems, users) = _dao.GetAllUsers(CurrentPage, RowsPerPage, Keyword, SortBy, SortOrder);
            if (totalItems >= 0)
            {
                Users = new FullObservableCollection<User>(users);
                TotalItems = totalItems;
                TotalPages = (TotalItems / RowsPerPage) + ((TotalItems % RowsPerPage == 0) ? 0 : 1);
            }
        }

        /// <summary>
        /// Inserts a new user into the database and adds it to the collection.
        /// </summary>
        /// <param name="user">The user to insert.</param>
        /// <returns>The number of rows affected.</returns>
        public int InsertUser(User user)
        {
            int count = _dao.InsertUser(user);

            if (count == 1)
            {
                Users.Add(user);
            }

            return count;
        }

        /// <summary>
        /// Counts the total number of courses.
        /// </summary>
        /// <returns>The total number of courses.</returns>
        public int CountCourse()
        {
            return _dao.CountCourse();
        }

        /// <summary>
        /// Loads the users for the specified page.
        /// </summary>
        /// <param name="page">The page number to load.</param>
        public void Load(int page)
        {
            CurrentPage = page;
            GetAllUsers();
        }

        /// <summary>
        /// Removes the specified user from the database.
        /// </summary>
        /// <param name="user">The user to remove.</param>
        public void RemoveUser(User user)
        {
            _dao.RemoveUserByID(user.Id);
        }

        /// <summary>
        /// Updates the specified user in the database.
        /// </summary>
        /// <param name="user">The user to update.</param>
        public void UpdateUser(User user)
        {
            _dao.UpdateUser(user);
        }
    }

}
