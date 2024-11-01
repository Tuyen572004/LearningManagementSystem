using LearningManagementSystem.ViewModels;
using System;

namespace LearningManagementSystem.Models
{
    public class Department : BaseViewModel
    {
        public int Id { get; set; }  // Auto-incrementing Id field
        public string DepartmentCode { get; set; }
        public string DepartmentDesc { get; set; }
    }
}
