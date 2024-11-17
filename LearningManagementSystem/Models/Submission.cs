using LearningManagementSystem.ViewModels;
using System;

namespace LearningManagementSystem.Models
{
    public class Submission : BaseViewModel
    {
        public int Id { get; set; }  // Auto-incrementing Id field
        public int AssignmentId { get; set; }  // Foreign key to Assignments table
        public int UserId { get; set; }  // Foreign key to Students table
        public DateTime SubmissionDate { get; set; }
        public string FilePath { get; set; }  // Path to the submitted file

        public string FileName { get; set; }// Original name of the file
        public string FileType { get; set;}  // MIME type or file extension


    }
}
