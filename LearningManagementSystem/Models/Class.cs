using System;

namespace LearningManagementSystem.Models
{
    public class Class : ICloneable
    {
        public int Id { get; set; }  // Auto-incrementing Id field
        public int CourseId { get; set; }  // Foreign key to Courses table

        public string ClassCode { get; set; }
        public DateTime ClassStartDate { get; set; }
        public DateTime ClassEndDate { get; set; }

        public object Clone()
        {
            return new Class
            {
                Id = this.Id,
                CourseId = this.CourseId,
                ClassCode = this.ClassCode,
                ClassStartDate = this.ClassStartDate,
                ClassEndDate = this.ClassEndDate
            };
        }

        // Navigation properties
        // public Course Course { get; set; }
        // public Cycle Cycle { get; set; }
    }
}
