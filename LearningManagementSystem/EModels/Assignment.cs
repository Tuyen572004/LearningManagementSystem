using System;
using System.Collections.Generic;

namespace LearningManagementSystem.EModels;

public partial class Assignment
{
    public int Id { get; set; }

    public int ClassId { get; set; }

    public int TeacherId { get; set; }

    public int? ResourceCategoryId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime DueDate { get; set; }

    public virtual Class Class { get; set; }

    public virtual Resourcecategory ResourceCategory { get; set; }

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();

    public virtual Teacher Teacher { get; set; }
}
