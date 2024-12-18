using LearningManagementSystem.DataAccess;
using System;
using System.Globalization;

namespace LearningManagementSystem.ViewModels
{
    public class HomePageViewModel
    {
        public DateTime datetime { get; set; }
        public string cultureNames { get; set; }

        public int TotalCourses { get; set; }

        public int TotalClasses { get; set; }
        public int TotalStudents { get; set; }
        public int TotalDepartments { get; set; }
        public CourseViewModel CrsViewModel { get; set; }

        public DepartmentsViewModel DpmViewModel { get; set; }

        public ClassViewModel ClsViewModel { get; set; }
        public StudentsInClassViewModel StdViewModel { get; set; }

        public HomePageViewModel()
        {
            datetime = new DateTime();
            cultureNames = "";
            DpmViewModel = new DepartmentsViewModel();
            CrsViewModel = new CourseViewModel();
            ClsViewModel = new ClassViewModel();

            TotalCourses = CrsViewModel.CountCourse();
            TotalDepartments = DpmViewModel.CountDepartments();
            TotalClasses = ClsViewModel.CountClasses();
            //   TotalStudents = StdViewModel.CountStudents()
        }

        public HomePageViewModel(string _cultureNames)
        {
            cultureNames = _cultureNames;
        }
        public void GetNow()
        {
            datetime = DateTime.Now;
        }

        public override string ToString()
        {
            return datetime.ToString("dddd, d MMM yyyy",
                  CultureInfo.CreateSpecificCulture(cultureNames));
        }

    }
}
