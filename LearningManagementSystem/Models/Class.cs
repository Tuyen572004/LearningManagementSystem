using System;

namespace LearningManagementSystem.Models
{
    public class Class
    {
        public int Id { get; set; }  // Auto-incrementing Id field
        public int CourseId { get; set; }  // Foreign key to Courses table
        public int CycleId { get; set; }   // Foreign key to Cycles table
        public DateTime ClassStartDate { get; set; }
        public DateTime ClassEndDate { get; set; }

        // Navigation properties
        // public Course Course { get; set; }
        // public Cycle Cycle { get; set; }
    }
}
