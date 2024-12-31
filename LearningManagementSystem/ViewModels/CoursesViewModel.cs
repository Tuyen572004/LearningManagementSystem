using LearningManagementSystem.Models;
using LearningManagementSystem.DataAccess;
using System.Collections.ObjectModel;
using System;
using LearningManagementSystem.Helpers;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;

namespace LearningManagementSystem.ViewModels
{
    /// <summary>
    /// ViewModel for managing courses in the Learning Management System.
    /// </summary>
    public class CourseViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the keyword for searching courses.
        /// </summary>
        public string Keyword { get; set; } = "";

        /// <summary>
        /// Gets or sets the sort order for displaying courses.
        /// </summary>
        public string SortOrder { get; set; } = "ASC";

        /// <summary>
        /// Gets or sets the field by which to sort courses.
        /// </summary>
        public string SortBy { get; set; } = "Id";

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
        /// Gets or sets the selected course.
        /// </summary>
        public Course SelectedCourse { get; set; }

        /// <summary>
        /// Gets or sets the collection of courses.
        /// </summary>
        public FullObservableCollection<Course> Courses { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CourseViewModel"/> class.
        /// </summary>
        public CourseViewModel()
        {
            _dao = App.Current.Services.GetService<IDao>();
            SelectedCourse = new Course();
            Courses = new FullObservableCollection<Course>();
            GetAllCourse();
        }

        /// <summary>
        /// Retrieves all courses based on the current filter and pagination settings.
        /// </summary>
        public void GetAllCourse()
        {
            var (totalItems, courses) = _dao.GetAllCourses(CurrentPage, PageSize, Keyword, SortBy, SortOrder);
            if (totalItems >= 0)
            {
                Courses = new FullObservableCollection<Course>(courses);
                TotalItems = totalItems;
                TotalPages = (TotalItems / PageSize) + ((TotalItems % PageSize == 0) ? 0 : 1);
            }
        }

        /// <summary>
        /// Inserts a new course into the database.
        /// </summary>
        /// <param name="course">The course to insert.</param>
        /// <returns>The number of rows affected.</returns>
        public int InsertCourse(Course course)
        {
            int count = _dao.InsertCourse(course);

            if (count == 1)
            {
                Courses.Add(course);
            }

            return count;
        }

        /// <summary>
        /// Counts the total number of courses in the database.
        /// </summary>
        /// <returns>The total number of courses.</returns>
        public int CountCourse()
        {
            return _dao.CountCourse();
        }

        /// <summary>
        /// Loads the courses for the specified page.
        /// </summary>
        /// <param name="page">The page number to load.</param>
        public void Load(int page)
        {
            CurrentPage = page;
            GetAllCourse();
        }

        /// <summary>
        /// Removes a course from the database.
        /// </summary>
        /// <param name="course">The course to remove.</param>
        /// <returns>The number of rows affected.</returns>
        public int RemoveCourse(Course course)
        {
            return _dao.RemoveCourseByID(course.Id);
        }

        /// <summary>
        /// Finds a course ID based on the input string.
        /// </summary>
        /// <param name="input">The input string containing the course code.</param>
        /// <returns>The course ID.</returns>
        public int FindCourseID(string input)
        {
            var token = input.Split(" - ");

            return _dao.FindCourseByID(new Course { CourseCode = token[0] });
        }

        /// <summary>
        /// Updates an existing course in the database.
        /// </summary>
        /// <param name="course">The course to update.</param>
        public void UpdateCourse(Course course)
        {
            _dao.UpdateCourse(course);
        }
    }

}