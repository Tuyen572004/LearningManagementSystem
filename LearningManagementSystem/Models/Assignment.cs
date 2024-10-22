using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Models
{
    internal class Assignment
    {
        public int Id { get; set; }
        public string CourseCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }

        // Navigation property
        //public Course Course { get; set; }
        // public ICollection<Submission> Submissions { get; set; }
    }
}
