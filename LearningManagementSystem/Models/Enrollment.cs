using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Models
{
    internal class Enrollment
    {
        public string CourseCode { get; set; }
        public string CycleCode { get; set; }
        public string StudentCode { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public bool Cancelled { get; set; }
        public string CancellationReason { get; set; }

        // Navigation properties
        //public Course Course { get; set; }
        //public Cycle Cycle { get; set; }
        //public Student Student { get; set; }
    }
}
