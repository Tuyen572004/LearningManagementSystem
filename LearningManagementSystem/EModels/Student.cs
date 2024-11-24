using System;
using System.Collections.Generic;

namespace LearningManagementSystem.EModels;

public partial class Student
{
    public int Id { get; set; }

    public string StudentCode { get; set; }

    public string StudentName { get; set; }

    public int EnrollmentYear { get; set; }

    public int? GraduationYear { get; set; }

    public string Email { get; set; }

    public DateTime BirthDate { get; set; }

    public string PhoneNo { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual User User { get; set; }
}
