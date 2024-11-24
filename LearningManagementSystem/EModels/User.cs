using System;
using System.Collections.Generic;

namespace LearningManagementSystem.EModels;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; }

    public string PasswordHash { get; set; }

    public string Email { get; set; }

    public string Role { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();

    public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
}
