﻿using LearningManagementSystem.Helper;
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
        public bool AddUser(User user)
        {
            throw new NotImplementedException();
        }

        public bool CheckUserInfo(User user)
        {
            throw new NotImplementedException();
        }

        public int CountCourse()
        {
            throw new NotImplementedException();
        }

        public int CountDepartments()
        {
            throw new NotImplementedException();
        }

        public void DeleteCourse(int courseId)
        {
            throw new NotImplementedException();
        }

        public List<ResourceCategory> findAllResourceCategories()
        {
            throw new NotImplementedException();
        }

        public List<BaseResource> findAssignmentsByClassId(int classId)
        {
            throw new NotImplementedException();
        }

        public Class findClassById(int classId)
        {
            throw new NotImplementedException();
        }

        public Course findCourseByClassId(int classId)
        {
            throw new NotImplementedException();
        }

        public int FindDepartmentID(Department department)
        {
            throw new NotImplementedException();
        }

        public List<BaseResource> findDocumentsByClassId(int classId)
        {
            throw new NotImplementedException();
        }

        public List<BaseResource> findNotificationsByClassId(int classId)
        {
            throw new NotImplementedException();
        }

        public Tuple<int, List<Course>> GetAllCourses(int page = 1, int pageSize = 10, string keyword = "", bool nameAscending = false)
        {
            var database = new List<Course>
            {
                new Course
                {
                    Id = 1,
                    CourseCode = "CSE101",
                    CourseDescription = "Introduction to Computer Science",
                    DepartmentId = 1
                },
                new Course
                {
                    Id = 2,
                    CourseCode = "CSE102",
                    CourseDescription = "Data Structures and Algorithms",
                    DepartmentId = 1
                },
                new Course
                {
                    Id = 3,
                    CourseCode = "CSE103",
                    CourseDescription = "Operating Systems",
                    DepartmentId = 1
                },
                new Course
                {
                    Id = 4,
                    CourseCode = "CSE104",
                    CourseDescription = "Computer Networks",
                    DepartmentId = 1
                },
                new Course
                {
                    Id = 5,
                    CourseCode = "CSE105",
                    CourseDescription = "Database Management Systems",
                    DepartmentId = 1
                },
                new Course
                {
                    Id = 6,
                    CourseCode = "CSE106",
                    CourseDescription = "Software Engineering",
                    DepartmentId = 1
                },
                new Course
                {
                    Id = 7,
                    CourseCode = "CSE107",
                    CourseDescription = "Web Development",
                    DepartmentId = 1
                },
                new Course
                {
                Id = 8,
                CourseCode = "CSE108",
                CourseDescription = "Computer Graphics",
                DepartmentId = 1
                }
            };

            var origin = database.Where(a => a.CourseDescription.Contains(keyword));
            if (nameAscending)
            {
                origin=origin.OrderBy(a => a.CourseCode);

            }

            var totalItems = origin.Count();

            var result = origin
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new Tuple<int, List<Course>>(totalItems, result);
        }

        public Tuple<int, List<Department>> GetAllDepartments(int page = 1, int pageSize = 10, string keyword = "", bool nameAscending = false)
        {
            throw new NotImplementedException();
        }

        public Course GetCourseById(int courseId)
        {
            Random random = new Random();
            courseId = random.Next(1, 20);
            if (courseId % 8 == 1)
            {
                return new Course
                {
                    Id = courseId,
                    CourseCode = "CSE101",
                    CourseDescription = "Introduction to Computer Science",
                    DepartmentId = 1
                };
            }
            if (courseId % 8 == 2)
            {
                return new Course
                {
                    Id = courseId,
                    CourseCode = "CSE102",
                    CourseDescription = "Data Structures and Algorithms",
                    DepartmentId = 1
                };
            }
            if (courseId % 8 == 3)
            {
                return new Course
                {
                    Id = courseId,
                    CourseCode = "CSE103",
                    CourseDescription = "Operating Systems",
                    DepartmentId = 1
                };
            }
            if (courseId % 8 == 4)
            {
                return new Course
                {
                    Id = courseId,
                    CourseCode = "CSE104",
                    CourseDescription = "Computer Networks",
                    DepartmentId = 1
                };
            }
            if (courseId % 8 == 5)
            {
                return new Course
                {
                    Id = courseId,
                    CourseCode = "CSE105",
                    CourseDescription = "Database Management Systems",
                    DepartmentId = 1
                };
            }
            if (courseId % 8 == 6)
            {
                return new Course
                {
                    Id = courseId,
                    CourseCode = "CSE106",
                    CourseDescription = "Software Engineering",
                    DepartmentId = 1
                };
            }
            if (courseId % 8 == 7)
            {
                return new Course
                {
                    Id = courseId,
                    CourseCode = "CSE107",
                    CourseDescription = "Web Development",
                    DepartmentId = 1
                };
            }

            return new Course
            {
                Id = courseId,
                CourseCode = "CSE108",
                CourseDescription = "Computer Graphics",
                DepartmentId = 1
            };

        }

        public Department GetDepartmentById(int departmentId)
        {
            Random random = new Random();
            departmentId = random.Next(1, 20);
            if (departmentId % 5 == 1)
            {
                return new Department
                {
                    Id = departmentId,
                    DepartmentCode = "CSE",
                    DepartmentDesc = "Computer Science and Engineering"
                };
            }
            if (departmentId % 5 == 2)
            {
                return new Department
                {
                    Id = departmentId,
                    DepartmentCode = "ECE",
                    DepartmentDesc = "Electronics and Communication Engineering"
                };
            }
            if (departmentId % 5 == 3)
            {
                return new Department
                {
                    Id = departmentId,
                    DepartmentCode = "EEE",
                    DepartmentDesc = "Electrical and Electronics Engineering"
                };
            }
            if (departmentId % 5 == 4)
            {
                return new Department
                {
                    Id = departmentId,
                    DepartmentCode = "CIV",
                    DepartmentDesc = "Civil Engineering"
                };
            }
            return new Department
            {
                Id = departmentId,
                DepartmentCode = "ME",
                DepartmentDesc = "Mechanical Engineering"
            };
        }

        public ObservableCollection<Class> GetEnrolledClassesByStudentId(int studentId)
        {
            return new ObservableCollection<Class>
            {
                    new Class
                {
                    Id = 1,
                    CourseId = 1,
                    ClassCode = "APCS_1",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 2,
                    CourseId = 2,
                    ClassCode = "APCS_2",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 3,
                    CourseId = 3,
                    ClassCode = "HP_1",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 4,
                    CourseId = 4,
                    ClassCode = "HP_2",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 5,
                    CourseId = 5,
                    ClassCode = "HP_3",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 6,
                    CourseId = 6,
                    ClassCode = "HP_4",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 7,
                    CourseId = 7,
                    ClassCode = "HP_5",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 8,
                    CourseId = 8,
                    ClassCode = "GP_1",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 9,
                    CourseId = 9,
                    ClassCode = "GP_2",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 10,
                    CourseId = 10,
                    ClassCode = "GP_3",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 11,
                    CourseId = 11,
                    ClassCode = "GP_4",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 12,
                    CourseId = 12,
                    ClassCode = "GP_5",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 13,
                    CourseId = 13,
                    ClassCode = "GP_6",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 14,
                    CourseId = 14,
                    ClassCode = "GP_7",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 15,
                    CourseId = 15,
                    ClassCode = "GP_8",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                }

            };
        }

        public Teacher GetTeacherById(int teacherId)
        {
            return new Teacher
            {
                Id = teacherId,
                TeacherCode = "T001",
                TeacherName = "John Doe",
                Email = "johndoe@example.com",
                PhoneNo = "1234567890",
                UserId = 1
            };
        }

        public FullObservableCollection<Teacher> GetTeachersByClassId(int classId)
        {
            return new FullObservableCollection<Teacher>
                {
                    new Teacher
                    {
                        Id = 1,
                        TeacherCode = "T001",
                        TeacherName = "John Doe",
                        Email = "johndoe@example.com",
                        PhoneNo = "1234567890",
                        UserId = 1
                    },
                    new Teacher
                    {
                        Id = 2,
                        TeacherCode = "T002",
                        TeacherName = "Jane Smith",
                        Email = "janesmith@example.com",
                        PhoneNo = "9876543210",
                        UserId = 2
                    },
                    new Teacher
                    {
                        Id = 3,
                        TeacherCode = "T003",
                        TeacherName = "Alice Johnson",
                        Email = "janesmith@example.com",
                        PhoneNo = "9876543210",
                        UserId = 3
                    }
            };
        }

        public int InsertCourse(Course course)
        {
            throw new NotImplementedException();
        }

        public int InsertDepartment(Department department)
        {
            throw new NotImplementedException();
        }

        public bool IsExistsUsername(string username)
        {
            throw new NotImplementedException();
        }

        public void RemoveCourseByID(int courseId)
        {
            throw new NotImplementedException();
        }

        public void RemoveDepartmentByID(int departmentId)
        {
            throw new NotImplementedException();
        }

        public void UpdateCourse(Course course)
        {
            throw new NotImplementedException();
        }

        public void UpdateDepartment(Department department)
        {
            throw new NotImplementedException();
        }

        
        FullObservableCollection<BaseResource> IDao.findAssignmentsByClassId(int classId)
        {
            throw new NotImplementedException();
        }

       

        FullObservableCollection<BaseResource> IDao.findDocumentsByClassId(int classId)
        {
            throw new NotImplementedException();
        }

        FullObservableCollection<BaseResource> IDao.findNotificationsByClassId(int classId)
        {
            throw new NotImplementedException();
        }
    }
}
