using System;

namespace LearningManagementSystem.Models
{
    internal class User
    {
        public int Id { get; set; }  // Auto-incrementing Id field
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } // 'student', 'teacher', 'admin'
        public DateTime CreatedAt { get; set; }
    }
}
