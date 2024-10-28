using LearningManagementSystem.Models;
using LearningManagementSystem.DataAccess;

using System.Collections.ObjectModel;

namespace LearningManagementSystem.ViewModels
{
    public class CourseViewModel: BaseViewModel
    {
        public Course Courses { get; set; }
        public int TotalSubjects { get; set; } = 0;
        public int TotalStudents { get; set; } = 0;
    }

    public class CourseTableViewModel: BaseViewModel
    {
        public int ID { get; set; }
        public string CourseCode { get; set; }
        public string CourseDescription { get; set; }

        public string DepartmentCode { get; set; }

        public int TotalStudents { get; set; }
        public int TotalSubjects { get; set; }

    }

    public class CoursesTableViewModel: BaseViewModel
    {
        private IDao _dao;

        public ObservableCollection<CourseTableViewModel> CoursesData { get; set; }

        public CoursesTableViewModel()
        {
            CoursesData = new ObservableCollection<CourseTableViewModel>();
            _dao = new MockDao();
        }

        public CoursesTableViewModel(IDao dao)
        {
            _dao = dao;
        }

        public void LoadCourses()
        { 
            for (int i=1; i<=8; i++)
            {
                var course=_dao.GetCourseById(i);

                //var totalSubjects = _dao.GetTotalSubjectsByCourseId(course.Id);
                //var totalStudents = _dao.GetTotalStudentsByCourseId(course.Id);

                CoursesData.Add(new CourseTableViewModel
                {
                    ID = course.Id,
                    CourseCode = course.CourseCode,
                    CourseDescription = course.CourseDescription,
                    DepartmentCode = _dao.GetDepartmentById(course.DepartmentId).DepartmentCode,
                    TotalStudents = 0,
                    TotalSubjects = 0

                });
                
            }
        }

    }
}
