using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Models
{
    internal class Notification
    {

        public int Id { get; set; }
        public string CourseCode { get; set; }
        public int TeacherId { get; set; }
        public string NotificationText { get; set; }
        public DateTime PostDate { get; set; }

        // Navigation properties
        //public Course Course { get; set; }
        //public Teacher Teacher { get; set; }

    }
}
