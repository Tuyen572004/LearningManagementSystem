using System;
using System.Collections.Generic;

namespace LearningManagementSystem.EModels;

public partial class Teachersperclass
{
    public int Id { get; set; }

    public int ClassId { get; set; }

    public int TeacherId { get; set; }

    public virtual Teacher Teacher { get; set; }
}
