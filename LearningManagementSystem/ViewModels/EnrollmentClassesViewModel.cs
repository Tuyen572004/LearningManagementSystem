using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helper;
using LearningManagementSystem.Models;
using LearningManagementSystem.Command;
using LearningManagementSystem.Views;

namespace LearningManagementSystem.ViewModels
{
    public class EnrollmentViewModel : BaseViewModel
    {
        public Class Class { get; set; }
        public Course Course { get; set; }
        public FullObservableCollection<Teacher> Teachers { get; set; }
        public Department Department { get; set; }
        
        public string ClassTitle => $"{Course.CourseCode}-{Course.CourseDescription} {Class.ClassCode}";
        public string DepartmentName => $"Dept. of {Department.DepartmentCode}-{Department.DepartmentDesc}";

        public ICommand NavigateToSpecificClassCommand { get; }

        public EnrollmentViewModel()
        {
            NavigateToSpecificClassCommand = new RelayCommand(NavigateToSpecificClass);
        }

        private void NavigateToSpecificClass()
        {
            NavigationService.NavigateTo("ClassDetailPage", this);
        }

    }
   
    public class EnrollmentClassesViewModel : BaseViewModel
    {
        private IDao _dao; // Private field to hold the dao instance
<<<<<<< Updated upstream
        public ObservableCollection<EnrollmentViewModel> enrolledClassesViewModel { get; set; }
=======
        public FullObservableCollection<EnrollmentViewModel> enrolledClassesViewModel { get; set; }
>>>>>>> Stashed changes

        public EnrollmentClassesViewModel() 
        {
            enrolledClassesViewModel = new FullObservableCollection<EnrollmentViewModel>();
            _dao = new MockDao();
        }

        public EnrollmentClassesViewModel(IDao dao) // Constructor accepting IDao
        {
            _dao = dao; // Assign the dao instance to the private field
        }

        public void LoadEnrolledClasses()
        {
            var studentId = 1; // Example student ID
            var enrolledClasses = _dao.GetEnrolledClassesByStudentId(studentId);

            foreach (var enrolledClass in enrolledClasses)
            {
                // get course, department
                var course = _dao.GetCourseById(enrolledClass.CourseId);
                var department = _dao.GetDepartmentById(course.DepartmentId);

                // get teachers
                var teachers = new FullObservableCollection<Teacher>(_dao.GetTeachersByClassId(enrolledClass.Id));

                enrolledClassesViewModel.Add(new EnrollmentViewModel
                {
                    Class = enrolledClass,
                    Course = course,
                    Department = department,
                    Teachers = teachers
                });
            }
        }
    }
}
