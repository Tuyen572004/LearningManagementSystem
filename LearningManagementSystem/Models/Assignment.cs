using Microsoft.UI.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Models
{
    public class Assignment : BaseResource
    {
        public int TeacherId { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }

    }
}
