using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningManagementSystem.Models;

namespace LearningManagementSystem.ViewModels
{
    public class EnrollmentClassesViewModel : BaseViewModel
    {
        private readonly object dao;

        public ObservableCollection<EnrolledClassViewModel> EnrolledClasses { get; set; }

        public EnrollmentClassesViewModel()
        {
            EnrolledClasses = new ObservableCollection<EnrolledClassViewModel>();
            LoadEnrolledClasses();
        }

        private void LoadEnrolledClasses()
        {
            var studentId = 1; // Example student ID
            var enrolledClasses = dao.GetEnrolledClassesByStudentId(studentId);

            foreach (var enrolledClass in enrolledClasses)
            {
                var className = dao.GetClassNameByClassId(enrolledClass.ClassId);
                var teacherId = dao.GetTeacherIdByClassId(enrolledClass.ClassId);
                var courseId = dao.GetCourseIdByClassId(enrolledClass.ClassId);
                var departmentId = dao.GetDepartmentIdByCourseId(courseId);
                var departmentName = dao.GetDepartmentNameByDepartmentId(departmentId);
                var teacherName = dao.GetTeacherNameByTeacherId(teacherId);

                EnrolledClasses.Add(new EnrolledClassViewModel
                {
                    ClassName = className,
                    TeacherName = teacherName,
                    DepartmentName = departmentName
                });
            }
        }
    }

    public class EnrolledClassViewModel : BaseViewModel
    {
        public string ClassName { get; set; }
        public string TeacherName { get; set; }
        public string DepartmentName { get; set; }
    }
}
