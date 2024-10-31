using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    public class HomePageViewModel
    {
        public DateTime datetime { get; set; }
        public string cultureNames { get; set; }

        public int TotalCourses { get; set; }

        public int TotalDepartments { get; set; }
        public CourseViewModel CrsViewModel { get; set; }

        public DepartmentsViewModel DpmViewModel { get; set; }


        public HomePageViewModel()
        {
            datetime = new DateTime();
            cultureNames = "";
            DpmViewModel = new DepartmentsViewModel();
            CrsViewModel = new CourseViewModel();
            TotalCourses = CrsViewModel.CountCourse();
            TotalDepartments = DpmViewModel.CountDepartments();
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
            return datetime.ToString("dddd, d MMM yyyy, HH:mm::ss",
                  CultureInfo.CreateSpecificCulture(cultureNames));
        }

    }
}
