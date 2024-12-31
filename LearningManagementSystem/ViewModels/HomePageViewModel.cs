using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Services.UserService;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;

namespace LearningManagementSystem.ViewModels
{
    /// <summary>
    /// ViewModel for the Home Page.
    /// </summary>
    public class HomePageViewModel
    {
        private IDao _dao;

        /// <summary>
        /// Gets or sets the current date and time.
        /// </summary>
        public DateTime datetime { get; set; }

        /// <summary>
        /// Gets or sets the culture names for formatting.
        /// </summary>
        public string cultureNames { get; set; }

        /// <summary>
        /// Gets or sets the total number of courses.
        /// </summary>
        public int TotalCourses { get; set; }

        /// <summary>
        /// Gets or sets the role of the current user.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets the total number of classes.
        /// </summary>
        public int TotalClasses { get; set; }

        /// <summary>
        /// Gets or sets the total number of students.
        /// </summary>
        public int TotalStudents { get; set; }

        /// <summary>
        /// Gets or sets the total number of departments.
        /// </summary>
        public int TotalDepartments { get; set; }

        /// <summary>
        /// Gets or sets the CourseViewModel instance.
        /// </summary>
        public CourseViewModel CrsViewModel { get; set; }

        /// <summary>
        /// Gets or sets the DepartmentsViewModel instance.
        /// </summary>
        public DepartmentsViewModel DpmViewModel { get; set; }

        /// <summary>
        /// Gets or sets the ClassViewModel instance.
        /// </summary>
        public ClassViewModel ClsViewModel { get; set; }

        //public StudentsInClassViewModel StdViewModel { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HomePageViewModel"/> class.
        /// </summary>
        public HomePageViewModel()
        {
            datetime = new DateTime();
            cultureNames = "";
            _dao = App.Current.Services.GetService<IDao>();
            DpmViewModel = new DepartmentsViewModel();
            CrsViewModel = new CourseViewModel();
            ClsViewModel = new ClassViewModel();
            TotalStudents = _dao.CountStudent();
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

        /// <summary>
        /// Initializes a new instance of the <see cref="HomePageViewModel"/> class with a specified culture name.
        /// </summary>
        /// <param name="_cultureNames">The culture names for formatting.</param>
        public HomePageViewModel(string _cultureNames)
        {
            cultureNames = _cultureNames;
        }

        /// <summary>
        /// Updates the datetime property to the current date and time.
        /// </summary>
        public void GetNow()
        {
            datetime = DateTime.Now;
        }

        /// <summary>
        /// Returns a string representation of the current date and time in the specified culture format.
        /// </summary>
        /// <returns>A string representation of the current date and time.</returns>
        public override string ToString()
        {
            return datetime.ToString("dddd, d MMM yyyy",
                  CultureInfo.CreateSpecificCulture(cultureNames));
        }
    }
}
