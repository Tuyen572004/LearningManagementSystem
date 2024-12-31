using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Enums;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using LearningManagementSystem.Services.UserService;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace LearningManagementSystem.ViewModels
{

    /// <summary>
    /// ViewModel for managing enrollment classes.
    /// </summary>
    public class EnrollmentClassesViewModel : BaseViewModel
    {
        private IDao _dao; // Private field to hold the dao instance
        public ObservableCollection<EnrollmentClassViewModel> enrolledClassesViewModel { get; set; }

        private UserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnrollmentClassesViewModel"/> class.
        /// </summary>
        public EnrollmentClassesViewModel()
        {
            enrolledClassesViewModel = new FullObservableCollection<EnrollmentClassViewModel>();
            _dao = App.Current.Services.GetService<IDao>(); ;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnrollmentClassesViewModel"/> class with a specified IDao.
        /// </summary>
        /// <param name="dao">The data access object to be used by this ViewModel.</param>
        public EnrollmentClassesViewModel(IDao dao) // Constructor accepting IDao
        {
            _dao = dao; // Assign the dao instance to the private field
        }

        /// <summary>
        /// Loads the enrolled classes for the current user based on their role.
        /// </summary>
        public void LoadEnrolledClasses()
        {
            var user = UserService.GetCurrentUser().Result;
            var role = user.Role;

            var enrolledClasses = new ObservableCollection<Class>();

            if (role.Equals(RoleEnum.GetStringValue(Role.Student)))
            {
                var student = _dao.GetStudentByUserId(user.Id);
                enrolledClasses = _dao.GetEnrolledClassesByStudentId(student.Id);
            }
            else if (role.Equals(RoleEnum.GetStringValue(Role.Teacher)))
            {
                var teacher = _dao.GetTeacherByUserId(user.Id);
                enrolledClasses = _dao.GetEnrolledClassesByTeacherId(teacher.Id);
            }

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
