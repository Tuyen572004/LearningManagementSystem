using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace LearningManagementSystem.ViewModels
{
    public class TableCoursesView : BaseViewModel, ICloneable
    {
        public int ID { get; set; }
        public string CourseCode { get; set; }
        public string CourseDecription { get; set; }
        public int TotalStudents { get; set; }
        public int TotalClasses { get; set; }
        public string DepartmentName { get; set; }
        public int DepartmentID { get; set; }
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
    public class TableCoursesViewModel : BaseViewModel
    {
        private IDao _dao = null;

        public TableCoursesView SelectedCourse { get; set; }
        public FullObservableCollection<TableCoursesView> TableCourses { get; set; }
        public List<string> Suggestion { get; set; }
        public string Keyword { get; set; } = "";
        public string SortBy { get; set; } = "Id";
        public string SortOrder { get; set; } = "ASC";
        public int CurrentPage { get; set; } = 1;
        public int RowsPerPage { get; set; } = 10;
        public int TotalPages { get; set; } = 0;
        public int TotalItems { get; set; } = 0;

        public int countRepeatButton { get; set; } = 0;
        public TableCoursesViewModel()
        {
            TableCourses = new FullObservableCollection<TableCoursesView>();
            _dao = App.Current.Services.GetService<IDao>(); ;
            countRepeatButton = 0;
            SelectedCourse = new TableCoursesView();
            Suggestion = _dao.GetAllCourseDecriptions();
        }
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

        public int InsertCourse(Course course)
        {
            int count = _dao.InsertCourse(course);

            if (count == 1)
            {
                //Courses.Add(course);
            }

            return count;
        }


        public void Load(int page = 1)
        {
            CurrentPage = page;
            GetAllCourse();
        }

        public void RemoveCourse(Course course)
        {
            _dao.RemoveCourseByID(course.Id);

        }
    }
}