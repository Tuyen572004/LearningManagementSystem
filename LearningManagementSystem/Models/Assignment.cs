using Microsoft.UI.Windowing;
using System;

namespace LearningManagementSystem.Models
{
    public class Assignment : 
    {
        public int TeacherId { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }

    }
}