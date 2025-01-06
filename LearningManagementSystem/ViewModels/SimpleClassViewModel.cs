#nullable enable
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Models;
using NPOI.OpenXmlFormats.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    /// <summary>
    /// ViewModel for managing simple class information.
    /// </summary>
    /// <param name="dao">The data access object for database operations.</param>
    /// <param name="classId">The ID of the class to manage.</param>
    partial class SimpleClassViewModel(IDao dao, int classId) : BaseViewModel
    {
        private readonly IDao _dao = dao;
        private readonly int _classId = classId;
        private Class? _managingClass = null;
        private Course? _associatedCourse = null;
        private Department? _associatedDepartment = null;

        /// <summary>
        /// Loads the required information for the class, course, and department.
        /// </summary>
        public void LoadRequiredInformation()
        {
            GetClassInformation();
            if (_managingClass is not null)
            {
                GetCourseInformation();
            }
            if (_associatedCourse is not null)
            {
                GetDepartmentInformation();
            }
        }

        /// <summary>
        /// Retrieves the class information from the database.
        /// </summary>
        private void GetClassInformation()
        {
            try
            {
                Class? retrievedClass = _dao.findClassById(_classId);
                _managingClass = retrievedClass;
            }
            catch (Exception)
            {
                // Do nothing for now
            }
        }

        /// <summary>
        /// Retrieves the course information associated with the class from the database.
        /// </summary>
        private void GetCourseInformation()
        {
            try
            {
                Course retrievedCourse = _dao.findCourseByClassId(_classId);
                _associatedCourse = retrievedCourse;
            }
            catch (Exception)
            {
                // Do nothing for now
            }
        }

        /// <summary>
        /// Retrieves the department information associated with the course from the database.
        /// </summary>
        private void GetDepartmentInformation()
        {
            try
            {
                Department retrievedDepartment = _dao.GetDepartmentById(_associatedCourse?.DepartmentId ?? -1);
                _associatedDepartment = retrievedDepartment;
            }
            catch (Exception)
            {
                // Do nothing for now
            }
        }

        /// <summary>
        /// Gets the class code.
        /// </summary>
        public string ClassCode => _managingClass?.ClassCode ?? "";

        /// <summary>
        /// Gets the course name.
        /// </summary>
        public string CourseName => _associatedCourse?.CourseDescription ?? "";

        /// <summary>
        /// Gets the detail page navigating parameter.
        /// </summary>
        /// <returns>An instance of <see cref="EnrollmentClassViewModel"/> if all required information is loaded; otherwise, null.</returns>
        public EnrollmentClassViewModel? DetailPageNavigatingParameter()
        {
            if (_managingClass is not null && _associatedCourse is not null && _associatedDepartment is not null)
            {
                return new EnrollmentClassViewModel()
                {
                    Class = _managingClass,
                    Course = _associatedCourse,
                    Department = _associatedDepartment
                };
            }
            return null;
        }
    }
}
