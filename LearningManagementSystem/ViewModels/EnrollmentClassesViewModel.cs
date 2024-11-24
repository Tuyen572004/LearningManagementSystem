using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using LearningManagementSystem.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace LearningManagementSystem.ViewModels
{
   
    public class EnrollmentClassesViewModel : BaseViewModel
    {
        private IDao _dao; // Private field to hold the dao instance
        public ObservableCollection<EnrollmentClassViewModel> enrolledClassesViewModel { get; set; }

        public EnrollmentClassesViewModel() 
        {
            enrolledClassesViewModel = new FullObservableCollection<EnrollmentClassViewModel>();
            _dao = new SqlDao(); 
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

                enrolledClassesViewModel.Add(new EnrollmentClassViewModel
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
