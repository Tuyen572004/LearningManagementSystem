using System;
using System.Collections.Generic;

namespace LearningManagementSystem.EModels;

public partial class Course
{
    public int Id { get; set; }

    public string CourseCode { get; set; }

    public string CourseDescription { get; set; }

    public int DepartmentId { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual Department Department { get; set; }
}
