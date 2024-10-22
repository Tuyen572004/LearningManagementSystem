using System;

namespace LearningManagementSystem.Models
{
    internal class Course
    {
        public int Id { get; set; }  // Auto-incrementing Id field
        public string CourseCode { get; set; }
        public string CourseDescription { get; set; }
        public int DepartmentId { get; set; }  // Foreign key to Departments table

        // Navigation property
        // public Department Department { get; set; }
    }
}
