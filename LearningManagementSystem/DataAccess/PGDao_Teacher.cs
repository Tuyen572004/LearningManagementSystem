using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.DataAccess
{
    public partial class SqlDao
    {
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
    }
}
