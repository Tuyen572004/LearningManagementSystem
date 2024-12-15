using LearningManagementSystem.Controls;
using LearningManagementSystem.Helpers;
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
    public partial interface IDao
    {
        public Tuple<int, List<Course>> GetAllCourses(int page = 1, int pageSize = 10, string keyword = "", string sortBy = "Id", string sortOrder = "ASC");

        List<string> GetAllCourseDecriptions();
        int InsertCourse(Course course);
        void UpdateCourse(Course course);
        void RemoveCourseByID(int courseId);

        int CountCourse();

        Tuple<int, List<Department>> GetAllDepartments(
            int page = 1,
            int pageSize = 10,
            string keyword = "",
            bool nameAscending = false
            );

        int InsertDepartment(Department department);
        void UpdateDepartment(Department department);
        void RemoveDepartmentByID(int departmentId);

        public void GetFullUser(User user);

        Tuple<int, List<User>> GetAllUsers(int page = 1, int pageSize = 10, string keyword = "", string sortBy = "Id", string sortOrder = "ASC");

        List<string> GetAllUsernames();
        int InsertUser(User user);
        void UpdateUser(User user);
        void RemoveUserByID(int userId);

        int CountUser();


        int CountDepartments();
        int FindDepartmentID(Department department);
        public ObservableCollection<Class> GetEnrolledClassesByStudentId(int studentId);
        public Course GetCourseById(int courseId);
        public Department GetDepartmentById(int departmentId);
        public FullObservableCollection<Teacher> GetTeachersByClassId(int classId);
        Teacher GetTeacherById(int teacherId);

        public bool CheckUserInfo(User user);

        public bool IsExistsUsername(string username);
        public bool AddUser(User user);

        public ObservableCollection<StudentVer2> GetStudentsByClassId(int classId);
        public (ObservableCollection<StudentVer2>, int) GetStudentsById(
            int ignoringCount = 0,
            int fetchingCount = 0,
            IEnumerable<int> chosenIds = null
            );
        public (ObservableCollection<StudentVer2>, int) GetStudents(
            bool fetchingAll = false,
            int ignoringCount = 0,
            int fetchingCount = 0,
            IEnumerable<int> chosenIds = null,
            IEnumerable<SortCriteria> sortCriteria = null,
            SearchCriteria searchCriteria = null,
            List<(StudentField field, object leftBound, object rightBound, bool containedLeftBound, bool withinBounds, bool containedRightBound)> filterCriteria = null
        )
        {
            return (null, 0);
        }
        public (
            IList<StudentVer2> addStudents,
            int addCount,
            IList<(StudentVer2 student, IEnumerable<String> error)> invalidStudentsInfo
            ) AddStudents(IEnumerable<StudentVer2> students);

        public (
            IList<StudentVer2> updateStudents,
            int updatedCount,
            IList<(StudentVer2 student, IEnumerable<string> error)> invalidStudentsInfo
            ) UpdateStudents(IEnumerable<StudentVer2> students);

        public (
            IList<StudentVer2> deleteStudents,
            int deletedCount,
            IList<(StudentVer2 student, IEnumerable<string> error)> invalidStudentsInfo
            ) DeleteStudents(IEnumerable<StudentVer2> students);


        public List<ResourceCategory> findAllResourceCategories();
        public FullObservableCollection<BaseResource> findNotificationsByClassId(int classId);

        public FullObservableCollection<BaseResource> findAssignmentsByClassId(int classId);

        public FullObservableCollection<BaseResource> findDocumentsByClassId(int classId);


        public Class findClassById(int classId);
        public Course findCourseByClassId(int classId);
        Submission GetSubmissionById(int id);
        Student GetStudentById(int studentId);
        Assignment GetAssignmentById(int assignmentId);
        Student GetStudentByUserId(int id);
        void SaveSubmission(Submission submission);
        List<Submission> GetSubmissionsByAssignmentId(int id);
        List<Submission> GetSubmissionsByAssignmentIdAndUserId(int id1, int id2);
        void UpdateSubmission(Submission submission);
        void DeleteSubmissionById(int id);

        void SaveAssignment(Assignment assignment);

        void UpdateAssignment(Assignment assignment);
        void DeleteAttachmentByAssignmentId(int id);
        void AddAssignment(Assignment assignment);
        void DeleteAssignmentById(int id);

    }
}
