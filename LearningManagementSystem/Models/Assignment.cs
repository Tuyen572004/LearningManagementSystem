using Microsoft.UI.Windowing;
using PropertyChanged;
using System;
using System.ComponentModel;

namespace LearningManagementSystem.Models
{
    
    public class Assignment : BaseResource
    {
        public int TeacherId { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }

        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }


    }
}