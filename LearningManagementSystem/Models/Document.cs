﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Models
{
    public class Document : BaseResource
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public string FileType { get; set; }
        public DateTime UploadDate { get; set; }
    }
}
