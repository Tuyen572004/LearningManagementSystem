using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LearningManagementSystem.ViewModels
{
    /// <summary>
    /// ViewModel for managing departments.
    /// </summary>
    public class DepartmentsViewModel
    {
        /// <summary>
        /// Gets or sets the keyword for searching departments.
        /// </summary>
        public string Keyword { get; set; } = "";

        /// <summary>
        /// Gets or sets a value indicating whether the department names should be sorted in ascending order.
        /// </summary>
        public bool NameAscending { get; set; } = false;

        /// <summary>
        /// Gets or sets the current page number for pagination.
        /// </summary>
        public int CurrentPage { get; set; } = 1;

        /// <summary>
        /// Gets or sets the number of items per page for pagination.
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Gets or sets the total number of pages available.
        /// </summary>
        public int TotalPages { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total number of items available.
        /// </summary>
        public int TotalItems { get; set; } = 0;

        private IDao _dao = null;

        /// <summary>
        /// Gets or sets the collection of departments.
        /// </summary>
        public FullObservableCollection<Department> Departments { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentsViewModel"/> class.
        /// </summary>
        public DepartmentsViewModel()
        {
            _dao = App.Current.Services.GetService<IDao>(); ;
            GetAllDepartments();
        }

        /// <summary>
        /// Counts the total number of departments.
        /// </summary>
        /// <returns>The total number of departments.</returns>
        public int CountDepartments()
        {
            return _dao.CountDepartments();
        }

        /// <summary>
        /// Finds the department ID based on the input string.
        /// </summary>
        /// <param name="input">The input string in the format "YY - XXXXX".</param>
        /// <returns>The department ID.</returns>
        public int FindDepartmentID(string input)
        {
            var token = input.Split(" - ");
            return _dao.FindDepartmentID(new Department { DepartmentCode = token[0] });
        }

        /// <summary>
        /// Retrieves all departments based on the current pagination and search criteria.
        /// </summary>
        public void GetAllDepartments()
        {
            var (totalItems, departments) = _dao.GetAllDepartments(CurrentPage, PageSize, Keyword, NameAscending);
            if (totalItems >= 0)
            {
                Departments = new FullObservableCollection<Department>(departments);
                TotalItems = totalItems;
                TotalPages = (TotalItems / PageSize) + ((TotalItems % PageSize == 0) ? 0 : 1);
            }
        }
    }
}
