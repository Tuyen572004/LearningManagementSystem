using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace LearningManagementSystem.ViewModels
{
    /// <summary>
    /// ViewModel for managing enrollment classes.
    /// </summary>
    public class EnrollmentClassViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the class information.
        /// </summary>
        public Class Class { get; set; }

        /// <summary>
        /// Gets or sets the course information.
        /// </summary>
        public Course Course { get; set; }

        /// <summary>
        /// Gets or sets the collection of teachers.
        /// </summary>
        public ObservableCollection<Teacher> Teachers { get; set; }

        /// <summary>
        /// Gets or sets the department information.
        /// </summary>
        public Department Department { get; set; }

        /// <summary>
        /// Gets the title of the class.
        /// </summary>
        public string ClassTitle => $"{Course.CourseCode}-{Course.CourseDescription} {Class.ClassCode}";

        /// <summary>
        /// Gets the name of the department.
        /// </summary>
        public string DepartmentName => $"Dept. of {Department.DepartmentCode}-{Department.DepartmentDesc}";

        /// <summary>
        /// Initializes a new instance of the <see cref="EnrollmentClassViewModel"/> class.
        /// </summary>
        public EnrollmentClassViewModel()
        {
            Class = new Class();
            Course = new Course();
            Department = new Department();
            Teachers = new ObservableCollection<Teacher>();
        }

        /// <summary>
        /// Loads the class information by class ID.
        /// </summary>
        /// <param name="classId">The ID of the class to load.</param>
        public void loadClassByClassId(int classId)
        {
            var dao = App.Current.Services.GetService<IDao>();
            Class = dao.findClassById(classId);
            Course = dao.GetCourseById(Class.CourseId);
            Department = dao.GetDepartmentById(Course.DepartmentId);
            Teachers = new ObservableCollection<Teacher>(dao.GetTeachersByClassId(classId));
        }
    }
}
