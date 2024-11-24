using System;
using System.Collections.Generic;

namespace LearningManagementSystem.EModels;

public partial class Cycle
{
    public int Id { get; set; }

    public string CycleCode { get; set; }

    public string CycleDescription { get; set; }

    public DateTime CycleStartDate { get; set; }

    public DateTime CycleEndDate { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
}
