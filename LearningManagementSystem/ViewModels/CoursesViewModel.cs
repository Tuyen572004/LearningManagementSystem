using LearningManagementSystem.Models;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helper;
using System.Collections.ObjectModel;
using System;

namespace LearningManagementSystem.ViewModels
{
    public class CourseViewModel : BaseViewModel
    {
        public string Keyword { get; set; } = "";
        public bool NameAscending { get; set; } = false;
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; } = 0;
        public int TotalItems { get; set; } = 0;

        private IDao _dao = null;

       
        public FullObservableCollection<Course> Courses { get; set; }

        public CourseViewModel()
        {
            _dao = new SqlDao();
            GetAllCourse();
        }

        public void GetAllCourse()
        {
            var (totalItems, courses) = _dao.GetAllCourses(CurrentPage, PageSize, Keyword, NameAscending);
            if (totalItems >= 0)
            {
                Courses = new FullObservableCollection<Course>(courses);
                TotalItems = totalItems;
                TotalPages = (TotalItems / PageSize) + ((TotalItems % PageSize == 0) ? 0 : 1);
            }
        }

        public int InsertCourse(Course course)
        {
            int count = _dao.InsertCourse(course);

            if (count == 1)
            {
                Courses.Add(course);
            }

            return count;
        }

        public void Load(int page)
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
