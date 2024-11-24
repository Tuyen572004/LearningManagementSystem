using System;
using System.Collections.Generic;

namespace LearningManagementSystem.EModels;

public partial class Notification
{
    public int Id { get; set; }

    public int ClassId { get; set; }

    public int? ResourceCategoryId { get; set; }

    public string Title { get; set; }

    public string NotificationText { get; set; }

    public DateTime? PostDate { get; set; }

    public virtual Class Class { get; set; }

    public virtual Resourcecategory ResourceCategory { get; set; }
}
