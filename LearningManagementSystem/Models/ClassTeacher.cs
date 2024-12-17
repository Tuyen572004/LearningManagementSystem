using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Models
{
    public class ClassTeacher
    {
        public int Id { get; set; }  // Auto-incrementing Id field
        public int ClassId { get; set; }  // Foreign key to Classes table
        public int TeacherId { get; set; } // Foreign key to Teachers table
    }
}
