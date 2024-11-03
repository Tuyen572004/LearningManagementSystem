using System;

namespace LearningManagementSystem.Models
{
    public enum EnrollmentState
    {
        Assigning,
        Cancelling,
        Withdrawing,
        // Finalizing,
    }
    public class Enrollment
    {
        public int Id { get; set; }  // Auto-incrementing Id field
        public int ClassId { get; set; }  // Foreign key to Classes table
        public int StudentId { get; set; } // Foreign key to Students table
        //public DateTime EnrollmentDate { get; set; }
        //public bool Cancelled { get; set; }
        //public string CancellationReason { get; set; }
        public EnrollmentState ActionType { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Note { get; set; }

        // Navigation properties
        // public Class Class { get; set; }
        // public Student Student { get; set; }
    }
}
