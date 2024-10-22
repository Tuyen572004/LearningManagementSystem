using System;

namespace LearningManagementSystem.Models
{
    internal class TeachersPerClass
    {
        public int Id { get; set; }  // Auto-incrementing Id field
        public int ClassId { get; set; }  // Foreign key to Classes table
        public int TeacherId { get; set; } // Foreign key to Teachers table

        // Navigation properties
        // public Class Class { get; set; }
        // public Teacher Teacher { get; set; }
    }
}
