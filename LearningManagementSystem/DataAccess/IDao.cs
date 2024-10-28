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
        Tuple<int, List<Course>> GetAllCourses(
            int page = 1,
            int pageSize = 10,
            string keyword = "",
            bool nameAscending = false
            );

        int InsertCourse(Course course);
        void UpdateCourse(Course course);
        void DeleteCourse(int courseId);

        public ObservableCollection<Class> GetEnrolledClassesByStudentId(int studentId);
        public Course GetCourseById(int courseId);
        public Department GetDepartmentById(int departmentId);
        public ObservableCollection<Teacher> GetTeachersByClassId(int classId);
        Teacher GetTeacherById(int teacherId);
    }
}
