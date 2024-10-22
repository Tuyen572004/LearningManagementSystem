using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Models
{
    internal class Document
    {
        public int Id { get; set; }
        public string CourseCode { get; set; }
        public string DocumentTitle { get; set; }
        public string DocumentPath { get; set; }
        public DateTime UploadDate { get; set; }

        // Navigation property
        //public Course Course { get; set; }
    }
}
