using System;
using System.Collections.Generic;

namespace LearningManagementSystem.EModels;

public partial class Teacher
{
    public int Id { get; set; }

    public string TeacherCode { get; set; }

    public string TeacherName { get; set; }

    public string Email { get; set; }

    public string PhoneNo { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual ICollection<Teachersperclass> Teachersperclasses { get; set; } = new List<Teachersperclass>();

    public virtual User User { get; set; }
}
