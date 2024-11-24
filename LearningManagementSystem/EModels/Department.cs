using System;
using System.Collections.Generic;

namespace LearningManagementSystem.EModels;

public partial class Department
{
    public int Id { get; set; }

    public string DepartmentCode { get; set; }

    public string DepartmentDesc { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
