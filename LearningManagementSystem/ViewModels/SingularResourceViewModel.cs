using LearningManagementSystem.Helper;
using LearningManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    public class SingularResourceViewModel : BaseViewModel
    {
        public ResourceCategory ResourceCategory { get; set; }
        public FullObservableCollection<BaseResource> Resources { get; set; }

        public SingularResourceViewModel(ResourceCategory resourceCategory)
        {
            ResourceCategory = resourceCategory;
            Resources = new FullObservableCollection<BaseResource>();
        }
    }
}
