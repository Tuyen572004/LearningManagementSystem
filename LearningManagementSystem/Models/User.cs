using System;
using System.Runtime.CompilerServices;

namespace LearningManagementSystem.Models
{
    //public enum Role
    //{
    //    Undetermined,
    //    Admin,
    //    Teacher,
    //    Student,
    //}
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } 
        // Consider changing to Role enum
        public DateTime CreatedAt { get; set; } 
        // What is this used for ??
        //public DateTime LastLogin { get; set; } // Maybe more useful than CreatedAt
    }
}
