using System;
using System.Collections.Generic;
using LearningManagementSystem.Helpers;
namespace LearningManagementSystem.EModels;

public partial class Course : PropertyChangedClass
{
    public int Id { get; set; }

    public string CourseCode { get; set; }

    public string CourseDescription { get; set; }

    public int DepartmentId { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual Department Department { get; set; }
}
