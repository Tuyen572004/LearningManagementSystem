using System;

namespace LearningManagementSystem.Models
{
    internal class Submission
    {
        public int Id { get; set; }  // Auto-incrementing Id field
        public int AssignmentId { get; set; }  // Foreign key to Assignments table
        public int StudentId { get; set; }  // Foreign key to Students table
        public DateTime SubmissionDate { get; set; }
        public string Answer { get; set; }

        // Navigation properties
        // public Assignment Assignment { get; set; }
        // public Student Student { get; set; }
    }
}
