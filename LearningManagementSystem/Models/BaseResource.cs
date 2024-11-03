using LearningManagementSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Models
{
    public class BaseResource : BaseViewModel
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int ResourceCategoryId { get; set; }

        public string Title { get; set; }
    }
}
