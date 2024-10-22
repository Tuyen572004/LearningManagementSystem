using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Models
{
    internal class Document
    {
        public int Id { get; set; }  // Auto-incrementing Id field
        public int ClassId { get; set; }  // Foreign key to Classes table
        public string DocumentTitle { get; set; }
        public string DocumentPath { get; set; }
        public DateTime UploadDate { get; set; }

        // Navigation property
        // public Class Class { get; set; }
    }
}
