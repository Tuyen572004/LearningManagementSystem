using LearningManagementSystem.Models;
using LearningManagementSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.DataAccess
{
    internal class MockDao : IDao
    {
        public ObservableCollection<Class> getEnrolledClasses(string studentId)
        {
            return new ObservableCollection<Class>()
            {
            new Class() { Id = 1, CourseId = 1, CycleId = 1, ClassStartDate = new DateTime(2021, 1, 1), ClassEndDate = new DateTime(2021, 1, 31) },
            new Class() { Id = 2, CourseId = 2, CycleId = 2, ClassStartDate = new DateTime(2021, 2, 1), ClassEndDate = new DateTime(2021, 2, 28) },
            new Class() { Id = 3, CourseId = 3, CycleId = 3, ClassStartDate = new DateTime(2021, 3, 1), ClassEndDate = new DateTime(2021, 3, 31) }
            };
        }

        public Teacher GetTeacherById(int teacherId)
        {
            return new Teacher() { Id = 1, TeacherName = "John Hoe", TeacherCode = "T01" };
        }

        public Department GetDepartmentById(int departmentId)
        {
            return new Department() { Id = 1,DepartmentDesc = "Computer Science", DepartmentCode = "CS" };
        }

        public ObservableCollection<Class> GetEnrolledClassesByStudentId(string studentId)
        {
            throw new NotImplementedException();
        }

        public string GetClassNameByClassId(int classId)
        {
            throw new NotImplementedException();
        }

        public int GetTeacherIdByClassId(int classId)
        {
            throw new NotImplementedException();
        }

        public int GetCourseIdByClassId(int classId)
        {
            throw new NotImplementedException();
        }

        public int GetDepartmentIdByCourseId(int courseId)
        {
            throw new NotImplementedException();
        }

        public string GetDepartmentNameByDepartmentId(int departmentId)
        {
            throw new NotImplementedException();
        }

        public string GetTeacherNameByTeacherId(int teacherId)
        {
            throw new NotImplementedException();
        }
    }
}
