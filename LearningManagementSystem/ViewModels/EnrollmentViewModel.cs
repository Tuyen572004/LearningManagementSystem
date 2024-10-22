using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    internal class EnrollmentViewModel : BaseViewModel
    {
        public string StudentCode { get; set; } // The code of the logged-in student
        public List<CourseEnrollment> EnrolledCourses { get; set; } // List of enrolled courses
    }
}
