using System;
using System.Collections.Generic;

namespace LearningManagementSystem.EModels;

public partial class Submission
{
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
