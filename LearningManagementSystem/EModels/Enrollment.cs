using System;
using System.Collections.Generic;

namespace LearningManagementSystem.EModels;

public partial class Enrollment
{
    public int Id { get; set; }

    public int ClassId { get; set; }

    public int StudentId { get; set; }

    public DateTime EnrollmentDate { get; set; }

    public virtual Class Class { get; set; }

    public virtual Student Student { get; set; }
}
