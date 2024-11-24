using System;
using System.Collections.Generic;

namespace LearningManagementSystem.EModels;

public partial class Class
{
    public int Id { get; set; }

    public string ClassCode { get; set; }

    public int CourseId { get; set; }

    public int CycleId { get; set; }

    public DateTime ClassStartDate { get; set; }

    public DateTime ClassEndDate { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual Course Course { get; set; }

    public virtual Cycle Cycle { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
