using System;

namespace LearningManagementSystem.Models
{
    internal class Cycle
    {
        public int Id { get; set; }  // Auto-incrementing Id field
        public string CycleCode { get; set; }
        public string CycleDescription { get; set; }
        public DateTime CycleStartDate { get; set; }
        public DateTime CycleEndDate { get; set; }
    }
}
