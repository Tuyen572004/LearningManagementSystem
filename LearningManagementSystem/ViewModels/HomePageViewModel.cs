using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Services.UserService;
using System;
using System.Globalization;

namespace LearningManagementSystem.ViewModels
{
    public class HomePageViewModel
    {
        public DateTime datetime { get; set; }
        public string cultureNames { get; set; }

        public int TotalCourses { get; set; }

        public string Role { get; set; }
        public int TotalClasses { get; set; }
        public int TotalStudents { get; set; }
        public int TotalDepartments { get; set; }
        public CourseViewModel CrsViewModel { get; set; }

        public DepartmentsViewModel DpmViewModel { get; set; }

        public ClassViewModel ClsViewModel { get; set; }
        //public StudentsInClassViewModel StdViewModel { get; set; }

        public HomePageViewModel()
        {
            datetime = new DateTime();
            cultureNames = "";
            DpmViewModel = new DepartmentsViewModel();
            CrsViewModel = new CourseViewModel();
            ClsViewModel = new ClassViewModel();
            var rawRole = UserService.GetCurrentUserRole().Result;
            if (rawRole == "admin")
            {
                Role = "Admin";
            }
            else if (rawRole == "teacher")
            {
                Role = "Teacher";
            }
            else if (rawRole == "student")
            {
                Role = "Student";
            }
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
