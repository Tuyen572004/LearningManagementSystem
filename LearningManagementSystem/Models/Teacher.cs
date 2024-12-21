#nullable enable
using LearningManagementSystem.ViewModels;
using System;

namespace LearningManagementSystem.Models
{
    public partial class Teacher : BaseViewModel
    {
        public int Id { get; set; }  // Auto-incrementing Id field
        required public string TeacherCode { get; set; }
        required public string TeacherName { get; set; }
        required public string Email { get; set; }
        public string? PhoneNo { get; set; } = null;
        public int? UserId { get; set; } = null;  // Foreign key to Users table

        // Navigation property
        // public User User { get; set; }
    }
}
