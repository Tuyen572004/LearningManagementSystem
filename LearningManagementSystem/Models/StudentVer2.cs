using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization.DateTimeFormatting;

namespace LearningManagementSystem.Models
{
    public class StudentVer2 : Student
    {
        //public int Id { get; set; }  // Auto-incrementing Id field
        //public string StudentCode { get; set; }
        //public string StudentName { get; set; }
        //public string Email { get; set; }
        //public DateTime BirthDate { get; set; }
        //public string PhoneNo { get; set; }
        //public int UserId { get; set; }  // Foreign key to Users table

        // Navigation property
        // public User User { get; set; }

        public int EnrollmentYear { get; set; }
        public int? GraduationYear { get; set; } = null;
    }
}
