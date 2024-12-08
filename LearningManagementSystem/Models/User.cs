using System;
using System.ComponentModel;
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
    public class User : INotifyPropertyChanged, ICloneable
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        // Consider changing to Role enum
        public DateTime CreatedAt { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public object Clone()
        {
            return new User
            {
                Id = this.Id,
                Username = this.Username,
                PasswordHash = this.PasswordHash,
                Email = this.Email,
                Role = this.Role,
                CreatedAt = this.CreatedAt
            };
        }

        // What is this used for ??
        //public DateTime LastLogin { get; set; } // Maybe more useful than CreatedAt
    }
}
