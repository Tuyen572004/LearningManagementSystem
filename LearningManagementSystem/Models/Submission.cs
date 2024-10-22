using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Models
{
    internal class Submission
    {

        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public string StudentCode { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string Answer { get; set; }

        // Navigation properties
        //public Assignment Assignment { get; set; }
        //public Student Student { get; set; }

    }
}
