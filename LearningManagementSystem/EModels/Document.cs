using System;
using System.Collections.Generic;

namespace LearningManagementSystem.EModels;

public partial class Document
{
    public int Id { get; set; }

    public int ClassId { get; set; }

    public int? ResourceCategoryId { get; set; }

    public string Title { get; set; }

    public string DocumentName { get; set; }

    public string DocumentPath { get; set; }

    public DateTime? UploadDate { get; set; }

    public virtual Class Class { get; set; }

    public virtual Resourcecategory ResourceCategory { get; set; }
}
