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
    class SimpleClassViewModel(IDao dao, int classId): BaseViewModel
    {
        private readonly IDao _dao = dao;
        private readonly int _classId = classId;
        private Class? _managingClass = null;
        private Course? _associatedCourse = null;

        public void LoadRequiredInformation()
        {
            GetClassInformation();
            if (_managingClass is not null)
            {
                GetCourseInformation();
            }
        }

        private void GetClassInformation()
        {
            try
            {
                Class? retrievedClass = null;
                _managingClass = retrievedClass;
            } catch (Exception)
            {

            }
        }

        private void GetCourseInformation()
        {
            try
            {
                Course retrievedCourse = _dao.GetCourseById(_managingClass?.CourseId ?? -1);
                _associatedCourse = retrievedCourse;
            } catch (Exception)
            {

            }
        }

        public string ClassCode => _managingClass?.ClassCode ?? "";
        public string CourseName => _associatedCourse?.CourseDescription ?? "";
    }
}
