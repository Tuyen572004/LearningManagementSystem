using System;
using System.Collections.Generic;

namespace LearningManagementSystem.EModels;

public partial class Resourcecategory
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Summary { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
