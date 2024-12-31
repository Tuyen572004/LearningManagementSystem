using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace LearningManagementSystem.ViewModels
{
    /// <summary>
    /// Represents a view model for table courses.
    /// </summary>
    public class TableCoursesView : BaseViewModel, ICloneable
    {
        /// <summary>
        /// Gets or sets the ID of the course.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the course code.
        /// </summary>
        public string CourseCode { get; set; }

        /// <summary>
        /// Gets or sets the course description.
        /// </summary>
        public string CourseDecription { get; set; }

        /// <summary>
        /// Gets or sets the total number of students enrolled in the course.
        /// </summary>
        public int TotalStudents { get; set; }

        /// <summary>
        /// Gets or sets the total number of classes for the course.
        /// </summary>
        public int TotalClasses { get; set; }

        /// <summary>
        /// Gets or sets the name of the department offering the course.
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// Gets or sets the ID of the department offering the course.
        /// </summary>
        public int DepartmentID { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return new TableCoursesView
            {
                ID = this.ID,
                CourseCode = this.CourseCode,
                CourseDecription = this.CourseDecription,
                DepartmentID = this.DepartmentID,
                TotalStudents = this.TotalStudents,
                TotalClasses = this.TotalClasses,
                DepartmentName = this.DepartmentName
            };
        }
    }
    /// <summary>
    /// ViewModel for managing table courses.
    /// </summary>
    public class TableCoursesViewModel : BaseViewModel
    {
        private IDao _dao = null;

        /// <summary>
        /// Gets or sets the selected course.
        /// </summary>
        public TableCoursesView SelectedCourse { get; set; }

        /// <summary>
        /// Gets or sets the collection of table courses.
        /// </summary>
        public FullObservableCollection<TableCoursesView> TableCourses { get; set; }

        /// <summary>
        /// Gets or sets the list of course description suggestions.
        /// </summary>
        public List<string> Suggestion { get; set; }

        /// <summary>
        /// Gets or sets the keyword for searching courses.
        /// </summary>
        public string Keyword { get; set; } = "";

        /// <summary>
        /// Gets or sets the field by which to sort the courses.
        /// </summary>
        public string SortBy { get; set; } = "Id";

        /// <summary>
        /// Gets or sets the sort order for the courses.
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
        /// Initializes a new instance of the <see cref="TableCoursesViewModel"/> class.
        /// </summary>
        public TableCoursesViewModel()
        {
            TableCourses = new FullObservableCollection<TableCoursesView>();
            _dao = App.Current.Services.GetService<IDao>(); ;
            countRepeatButton = 0;
            SelectedCourse = new TableCoursesView();
            Suggestion = _dao.GetAllCourseDecriptions();
        }

        /// <summary>
        /// Retrieves all courses based on the current filter and pagination settings.
        /// </summary>
        public void GetAllCourse()
        {
            var (totalItems, courses) = _dao.GetAllCourses(CurrentPage, RowsPerPage, Keyword, SortBy, SortOrder);
            if (courses != null)
            {
                TableCourses.Clear();
                for (int i = 0; i < courses.Count; i++)
                {
                    TableCourses.Add(new TableCoursesView
                    {
                        ID = courses[i].Id,
                        CourseCode = courses[i].CourseCode,
                        CourseDecription = courses[i].CourseDescription,
                        DepartmentID = courses[i].DepartmentId,
                        DepartmentName = _dao.GetDepartmentById(courses[i].DepartmentId).DepartmentDesc,
                        TotalStudents = _dao.findTotalStudentsByCourseId(courses[i].Id),
                        TotalClasses = _dao.findTotalClassesByCourseId(courses[i].Id)
                    });
                }
                TotalItems = totalItems;
                TotalPages = (TotalItems / RowsPerPage)
                    + ((TotalItems % RowsPerPage == 0)
                            ? 0 : 1);
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
                //Courses.Add(course);
            }

            return count;
        }

        /// <summary>
        /// Loads the courses for the specified page.
        /// </summary>
        /// <param name="page">The page number to load.</param>
        public void Load(int page = 1)
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
    }
}