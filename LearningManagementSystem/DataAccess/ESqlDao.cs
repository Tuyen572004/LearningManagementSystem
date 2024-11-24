using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using LearningManagementSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LearningManagementSystem.DataAccess
{
    public class ESqlDao : IDao
    {
        private readonly LmsdbContext _context;

        public ESqlDao()
        {
            _context = new LmsdbContext();
            if (!_context.Database.CanConnectAsync().Result)
            {
                throw new Exception("Cannot connect to database");
            }
        }

        public bool AddUser(User user)
        {
            _context.Users.Add(user);
            return _context.SaveChanges() > 0;
        }

        public bool CheckUserInfo(User user)
        {
            return _context.Users.Any(u => u.Username == user.Username && u.PasswordHash == user.PasswordHash);
        }

        public int CountCourse()
        {
            return _context.Courses.Count();
        }

        public int CountDepartments()
        {
            return _context.Departments.Count();
        }

        public void DeleteSubmissionById(int id)
        {
            var submission = _context.Submissions.Find(id);
            if (submission != null)
            {
                _context.Submissions.Remove(submission);
                _context.SaveChanges();
            }
        }

        public List<ResourceCategory> findAllResourceCategories()
        {
            return _context.Resourcecategories.ToList();
        }

        public FullObservableCollection<BaseResource> findAssignmentsByClassId(int classId)
        {
            // Implement this method based on your specific requirements
            throw new NotImplementedException();
        }

        public Class findClassById(int classId)
        {
            return _context.Classes.Find(classId);
        }

        public Course findCourseByClassId(int classId)
        {
            return _context.Courses.FirstOrDefault(c => c.Classes.Any(cl => cl.Id == classId));
        }

        public int FindDepartmentID(Department department)
        {
            var dept = _context.Departments.FirstOrDefault(d => d.DepartmentCode == department.DepartmentCode);
            return dept?.Id ?? 0;
        }

        public FullObservableCollection<BaseResource> findDocumentsByClassId(int classId)
        {
            // Implement this method based on your specific requirements
            throw new NotImplementedException();
        }

        public FullObservableCollection<BaseResource> findNotificationsByClassId(int classId)
        {
            // Implement this method based on your specific requirements
            throw new NotImplementedException();
        }

        public Tuple<int, List<Course>> GetAllCourses(int page = 1, int pageSize = 10, string keyword = "", bool nameAscending = false)
        {
            var query = _context.Courses.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(c => c.CourseCode.Contains(keyword) || c.CourseDescription.Contains(keyword));
            }
            if (nameAscending)
            {
                query = query.OrderBy(c => c.CourseCode);
            }
            else
            {
                query = query.OrderByDescending(c => c.CourseCode);
            }
            var total = query.Count();
            var courses = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return new Tuple<int, List<Course>>(total, courses);
        }

        public Tuple<int, List<Department>> GetAllDepartments(int page = 1, int pageSize = 10, string keyword = "", bool nameAscending = false)
        {
            var query = _context.Departments.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(d => d.DepartmentCode.Contains(keyword) || d.DepartmentDesc.Contains(keyword));
            }
            if (nameAscending)
            {
                query = query.OrderBy(d => d.DepartmentCode);
            }
            else
            {
                query = query.OrderByDescending(d => d.DepartmentCode);
            }
            var total = query.Count();
            var departments = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return new Tuple<int, List<Department>>(total, departments);
        }

        public Assignment GetAssignmentById(int assignmentId)
        {
            return _context.Assignments.Find(assignmentId);
        }

        public Course GetCourseById(int courseId)
        {
            return _context.Courses.Find(courseId);
        }

        public Department GetDepartmentById(int departmentId)
        {
            return _context.Departments.Find(departmentId);
        }

        public ObservableCollection<Class> GetEnrolledClassesByStudentId(int studentId)
        {
            var classes = _context.Enrollments.Where(e => e.StudentId == studentId).Select(e => e.Class).ToList();
            return new ObservableCollection<Class>(classes);
        }

        public Student GetStudentById(int studentId)
        {
            return _context.Students.Find(studentId);
        }

        public Student GetStudentByUserId(int id)
        {
            return _context.Students.FirstOrDefault(s => s.UserId == id);
        }

        public (ObservableCollection<StudentVer2>, int) GetStudents(bool fetchingAll = false, int ignoringCount = 0, int fetchingCount = 0, List<(StudentField field, Ordering order)> sortCriteria = null, List<(StudentField field, object keyword)> searchKeyword = null, List<(StudentField field, object leftBound, object rightBound, bool containedLeftBound, bool withinBounds, bool containedRightBound)> filterCriteria = null)
        {
            // Implement this method based on your specific requirements
            throw new NotImplementedException();
        }

        public ObservableCollection<StudentVer2> GetStudentsByClassId(int classId)
        {
            // Implement this method based on your specific requirements
            throw new NotImplementedException();
        }

        public (ObservableCollection<StudentVer2>, int) GetStudentsById(int ignoringCount = 0, int fetchingCount = 0, IEnumerable<int> chosenIds = null)
        {
            // Implement this method based on your specific requirements
            throw new NotImplementedException();
        }

        public Submission GetSubmissionById(int id)
        {
            return _context.Submissions.Find(id);
        }

        public List<Submission> GetSubmissionsByAssignmentId(int id)
        {
            return _context.Submissions.Where(s => s.AssignmentId == id).ToList();
        }

        public List<Submission> GetSubmissionsByAssignmentIdAndUserId(int id1, int id2)
        {
            return _context.Submissions.Where(s => s.AssignmentId == id1 && s.UserId == id2).ToList();
        }

        public Teacher GetTeacherById(int teacherId)
        {
            return _context.Teachers.Find(teacherId);
        }

        public FullObservableCollection<Teacher> GetTeachersByClassId(int classId)
        {
            // Implement this method based on your specific requirements
            throw new NotImplementedException();
        }

        public int InsertCourse(Course course)
        {
            _context.Courses.Add(course);
            _context.SaveChanges();
            return course.Id;
        }

        public int InsertDepartment(Department department)
        {
            _context.Departments.Add(department);
            _context.SaveChanges();
            return department.Id;
        }

        public bool IsExistsUsername(string username)
        {
            return _context.Users.Any(u => u.Username == username);
        }

        public void RemoveCourseByID(int courseId)
        {
            var course = _context.Courses.Find(courseId);
            if (course != null)
            {
                _context.Courses.Remove(course);
                _context.SaveChanges();
            }
        }

        public void RemoveDepartmentByID(int departmentId)
        {
            var department = _context.Departments.Find(departmentId);
            if (department != null)
            {
                _context.Departments.Remove(department);
                _context.SaveChanges();
            }
        }

        public void SaveSubmission(Submission submission)
        {
            _context.Submissions.Add(submission);
            _context.SaveChanges();
        }

        public void UpdateCourse(Course course)
        {
            _context.Courses.Update(course);
            _context.SaveChanges();
        }

        public void UpdateDepartment(Department department)
        {
            _context.Departments.Update(department);
            _context.SaveChanges();
        }

        public void UpdateSubmission(Submission submission)
        {
            _context.Submissions.Update(submission);
            _context.SaveChanges();
        }
    }
}
