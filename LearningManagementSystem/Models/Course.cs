using System;
using System.ComponentModel;

namespace LearningManagementSystem.Models
{
    public class Course : INotifyPropertyChanged
    {
        public int Id { get; set; }  // Auto-incrementing Id field
        public string CourseCode { get; set; }
        public string CourseDescription { get; set; }
        public int DepartmentId { get; set; }  // Foreign key to Departments table

        public event PropertyChangedEventHandler PropertyChanged;

        // Navigation property
        // public Department Department { get; set; }
    }
}
