using LearningManagementSystem.Controls;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using LearningManagementSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LearningManagementSystem.DataAccess
{
    public partial interface IDao
    {
        public Tuple<int, List<Course>> GetAllCourses(int page = 1, int pageSize = 10, string keyword = "", string sortBy = "Id", string sortOrder = "ASC");

        List<string> GetAllCourseDecriptions();
        int InsertCourse(Course course);
        void UpdateCourse(Course course);
        int RemoveCourseByID(int courseId);

        int CountCourse();

        Tuple<int, List<Department>> GetAllDepartments(
            int page = 1,
            int pageSize = 10,
            string keyword = "",
            bool nameAscending = false
            );
        public int findTotalClassesByCourseId(int courseId);
        public int findTotalStudentsByCourseId(int courseId);
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
        int FindCourseByID(Course course);
        public ObservableCollection<Class> GetEnrolledClassesByStudentId(int studentId);
        public Course GetCourseById(int courseId);
        public Department GetDepartmentById(int departmentId);
        public FullObservableCollection<Teacher> GetTeachersByClassId(int classId);
        Teacher GetTeacherById(int teacherId);

        public (
            IList<Teacher> addedTeachers,
            int addedCount,
            IList<(Teacher teacher, IEnumerable<string> errors)> invalidTeachersInfo
        ) AddTeachers(IEnumerable<Teacher> teachers);

        public (
           IList<Teacher> updatedTeachers,
           int updatedCount,
           IList<(Teacher teacher, IEnumerable<string> errors)> invalidTeachersInfo
       ) UpdateTeachers(IEnumerable<Teacher> teachers);

        public (
            IList<Teacher> deletedTeachers,
            int deletedCount,
            IList<(Teacher teacher, IEnumerable<string> errors)> invalidTeachersInfo
        ) DeleteTeachers(IEnumerable<Teacher> teachers);

        public (ObservableCollection<Teacher>, int) GetTeachers(
            bool fetchingAll = false,
            int ignoringCount = 0,
            int fetchingCount = 0,
            IEnumerable<int> chosenIds = null,
            IEnumerable<SortCriteria> sortCriteria = null,
            SearchCriteria searchCriteria = null
        );

        public bool CheckUserInfo(User user);

        public bool IsExistsUsername(string username);
        public bool AddUser(User user);

        //public ObservableCollection<StudentVer2> GetStudentsByClassId(int classId);
        //public (ObservableCollection<StudentVer2>, int) GetStudentsById(
        //    int ignoringCount = 0,
        //    int fetchingCount = 0,
        //    IEnumerable<int> chosenIds = null
        //    );
        public (ObservableCollection<StudentVer2>, int) GetStudents(
            bool fetchingAll = false,
            int ignoringCount = 0,
            int fetchingCount = 0,
            IEnumerable<int> chosenIds = null,
            IEnumerable<SortCriteria> sortCriteria = null,
            SearchCriteria searchCriteria = null
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

        public (ObservableCollection<StudentVer2>, int) GetStudentsFromClass(
            int classId,
            bool fetchingAll = false,
            int ignoringCount = 0,
            int fetchingCount = 0,
            IEnumerable<SortCriteria> sortCriteria = null
        );

        public (
            IList<int> addedStudentIds,
            int addedCount,
            IList<(int studentId, IEnumerable<string> error)> invalidStudentIdsInfo
        ) AddStudentsToClass(IEnumerable<int> studentIds, int classId);

        public (
            IList<int> removedStudentIds,
            int removedCount,
            IList<(int studentId, IEnumerable<string> error)> invalidStudentIdsInfo
        ) RemoveStudentsFromClass(IEnumerable<int> studentIds, int classId);

        public (ObservableCollection<Teacher>, int) GetTeachersFromClass(
            int classId,
            bool fetchingAll = false,
            int ignoringCount = 0,
            int fetchingCount = 0,
            IEnumerable<SortCriteria> sortCriteria = null
        );

        public (
            IList<int> addedTeacherIds,
            int addedCount,
            IList<(int teacherId, IEnumerable<string> error)> invalidTeacherIdsInfo
        ) AddTeachersToClass(IEnumerable<int> teacherIds, int classId);

        public (
            IList<int> removedTeacherIds,
            int removedCount,
            IList<(int teacherId, IEnumerable<string> error)> invalidTeacherIdsInfo
        ) RemoveTeachersFromClass(IEnumerable<int> teacherIds, int classId);

        public List<ResourceCategory> findAllResourceCategories();
        public FullObservableCollection<BaseResource> findNotificationsByClassId(int classId);

        public FullObservableCollection<BaseResource> findAssignmentsByClassId(int classId);

        public int InsertClass(Class newClass);
        public Class findClassById(int classId);
        public Course findCourseByClassId(int classId);
        Submission GetSubmissionById(int id);
        Assignment GetAssignmentById(int assignmentId);
        Student GetStudentByUserId(int id);
        void SaveSubmission(Submission submission);
        List<Submission> GetSubmissionsByAssignmentId(int id);
        List<Submission> GetSubmissionsByAssignmentIdAndUserId(int id1, int id2);
        void UpdateSubmission(Submission submission);
        void DeleteSubmissionById(int id);

        void SaveAssignment(Assignment assignment);

        public User GetUserByUserName(string username);
        void UpdateAssignment(Assignment assignment);
        void DeleteAttachmentByAssignmentId(int id);
        void AddAssignment(Assignment assignment);
        void DeleteAssignmentById(int id);

        public List<String> GetAllClassesCode();
        public Tuple<int, List<Class>> GetAllClasses(int page = 1, int pageSize = 10, string keyword = "", string sortBy = "Id", string sortOrder = "ASC");
        public void RemoveClassByID(int id);
        public void UpdateClass(Class newClass);
        public int CountClass();
        public int CountStudent();
    }
}
