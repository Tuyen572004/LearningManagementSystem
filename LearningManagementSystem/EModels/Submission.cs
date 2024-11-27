using LearningManagementSystem.Helpers;
using System;

namespace LearningManagementSystem.EModels;

public partial class Submission: PropertyChangedClass
{

    public Submission(Submission submission)
    {
        this.Id = submission.Id;
        this.AssignmentId = submission.AssignmentId;
        this.UserId = submission.UserId;
        this.SubmissionDate = submission.SubmissionDate;
        this.FilePath = submission.FilePath;
        this.FileName = submission.FileName;
        this.FileType = submission.FileType;
        this.Grade = submission.Grade;
        this.Assignment = submission.Assignment;
        this.User = submission.User;
    }

    public int Id { get; set; }

    public int AssignmentId { get; set; }

    public int UserId { get; set; }

    public DateTime? SubmissionDate { get; set; }

    public string FilePath { get; set; }

    public string FileName { get; set; }

    public string FileType { get; set; }

    public double? Grade { get; set; }

    public virtual Assignment Assignment { get; set; }

    public virtual User User { get; set; }
}
