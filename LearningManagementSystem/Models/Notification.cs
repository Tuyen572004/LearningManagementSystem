using System;

namespace LearningManagementSystem.Models
{
    public class Notification : BaseResource
    {
        public int CreatedBy { get; set; }

        public string Description { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public DateTime PostDate { get; set; }
    }
}
