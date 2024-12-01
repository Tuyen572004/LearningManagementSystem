using LearningManagementSystem.ViewModels;
using System;

namespace LearningManagementSystem.Models
{
    public class Submission : BaseViewModel
    {
        public int Id { get; set; }  // Auto-incrementing Id field
        public int AssignmentId { get; set; }  
        public int UserId { get; set; }  
        public DateTime SubmissionDate { get; set; }
        public string FilePath { get; set; }  // Path to the submitted file on the server

        public string FileName { get; set; }// Original name of the file
        public string FileType { get; set;}  // MIME type or file extension

        public Decimal? Grade { get; set; } 
    }
}
