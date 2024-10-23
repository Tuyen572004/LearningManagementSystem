using System;

namespace LearningManagementSystem.Models
{
    public class Teacher
    {
        public int Id { get; set; }  // Auto-incrementing Id field
        public string TeacherCode { get; set; }
        public string TeacherName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public int UserId { get; set; }  // Foreign key to Users table

        // Navigation property
        // public User User { get; set; }
    }
}
