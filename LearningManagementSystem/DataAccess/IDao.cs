using LearningManagementSystem.Models;
using LearningManagementSystem.ViewModels;
using Microsoft.UI.Composition;
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
        void RemoveCourseByID(int courseId);

        int CountCourse();
        // ------------------------------------------------ //

        Tuple<int, List<Department>> GetAllDepartments(
            int page = 1,
            int pageSize = 10,
            string keyword = "",
            bool nameAscending = false
            );

        int InsertDepartment(Department department);
        void UpdateDepartment(Department department);
        void RemoveDepartmentByID(int departmentId);

        int CountDepartments();
        int FindDepartmentID(Department department);
        // ------------------------------------------------ //
        public ObservableCollection<Class> GetEnrolledClassesByStudentId(int studentId);
        public Course GetCourseById(int courseId);
        public Department GetDepartmentById(int departmentId);
        public ObservableCollection<Teacher> GetTeachersByClassId(int classId);
        Teacher GetTeacherById(int teacherId);

        // ------------------------------------------------ //
        public bool CheckUserInfo(User user);

        public bool IsExistsUsername(string username);
        public bool AddUser(User user);

        // ------------------------------------------------ //



        public ObservableCollection<StudentVer2> GetStudentsByClassId(int classId)
        {
            return [];
        }
        public ObservableCollection<StudentVer2> GetStudentsByIds(IEnumerable<int> studentIds)
        {
            return [];
        }
        public (ObservableCollection<StudentVer2>, int) GetStudents(
            bool fetchingAll = false,
            int ignoringCount = 0,
            int fetchingCount = 0,
            List<(StudentField, Ordering)> sortCriteria = null,
            List<(StudentField, object)> searchKeyword = null,
            List<(StudentField, object, object, bool, bool, bool)> filterCriteria = null
            )
        {
            // temporary body, so that other IDao don't have to implement this method yet
            var studentsToo = DataProvider.StudentList();

            var filteredEnumerable = studentsToo.AsEnumerable();

            // Search
            foreach ((StudentField field, object keyword) in searchKeyword)
            {
                filteredEnumerable = filteredEnumerable.Where(student =>
                {
                    if (keyword is null && student.GetValueByField(field) is null)
                    {
                        return true;
                    }            
                     
                    if (keyword is null || student.GetValueByField(field) is null)
                    {
                        return false;
                    }

                    try
                    {
                        // Try to convert object to string, then compare them
                        return (student.GetValueByField(field) as string).Contains(keyword as string);
                    }
                    catch (InvalidCastException)
                    {
                        // If failed, then compare them directly, and hope it works
                        return student.GetValueByField(field).Equals(keyword);
                    }
                });
                    
            }

            // Filter
            filteredEnumerable = filteredEnumerable.ConditionallyFiltered(filterCriteria);

            // Sort
            // SQL: Just use ORDER BY :'))
            foreach ((StudentField field, Ordering order) in sortCriteria.AsEnumerable().Reverse())
            {
                if (order == Ordering.Ascending)
                {
                    filteredEnumerable = filteredEnumerable.OrderBy(student => student.GetValueByField(field));
                }
                else
                {
                    filteredEnumerable = filteredEnumerable.OrderByDescending(student => student.GetValueByField(field));
                }
            }

            if (fetchingAll)
            {
                return (new ObservableCollection<StudentVer2>(filteredEnumerable), filteredEnumerable.Count());
            }

            var queryTotal = filteredEnumerable.Count();
            filteredEnumerable = filteredEnumerable.Skip(ignoringCount).Take(fetchingCount);
            return (new ObservableCollection<StudentVer2>(filteredEnumerable), queryTotal);
        }
    }
}
