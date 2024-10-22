using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Models
{
    internal class Assignment
    {
        public int Id { get; set; }  // Auto-incrementing Id field
        public int ClassId { get; set; }  // Foreign key to Classes table
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }

        // Navigation property
        // public Class Class { get; set; }
    }
}
