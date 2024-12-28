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
    partial class SimpleClassViewModel(IDao dao, int classId) : BaseViewModel
    {
        private readonly IDao _dao = dao;
        private readonly int _classId = classId;
        private Class? _managingClass = null;
        private Course? _associatedCourse = null;
        private Department? _associatedDepartment = null;

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

        private void GetClassInformation()
        {
            try
            {
                Class? retrievedClass = _dao.findClassById(_classId);
                _managingClass = retrievedClass;
            } catch (Exception)
            {
                // No nothing for now
            }
        }

        private void GetCourseInformation()
        {
            try
            {
                Course retrievedCourse = _dao.findCourseByClassId(_managingClass?.CourseId ?? -1);
                _associatedCourse = retrievedCourse;
            } catch (Exception)
            {
                // No nothing for now
            }
        }

        private void GetDepartmentInformation()
        {
            try
            {
                Department retrievedDepartment = _dao.GetDepartmentById(_associatedCourse?.DepartmentId ?? -1);
                _associatedDepartment = retrievedDepartment;
            }
            catch (Exception)
            {
                // No nothing for now
            }
        }

        public string ClassCode => _managingClass?.ClassCode ?? "";
        public string CourseName => _associatedCourse?.CourseDescription ?? "";

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
