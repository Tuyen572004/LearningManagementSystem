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
    public interface IDao
    {
        public ObservableCollection<Class> GetEnrolledClassesByStudentId(string studentId);
        public string GetClassNameByClassId(int classId);
        public int GetTeacherIdByClassId(int classId);
        public int GetCourseIdByClassId(int classId);
        public int GetDepartmentIdByCourseId(int courseId);
        public string GetDepartmentNameByDepartmentId(int departmentId);
        public string GetTeacherNameByTeacherId(int teacherId);
        
    }
}
