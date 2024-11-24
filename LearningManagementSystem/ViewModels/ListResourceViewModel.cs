using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{

    public class ListResource : BaseViewModel
    {
        public FullObservableCollection<SingularResourceViewModel> SingularResources { get; set; }

        public ListResource()
        {
            SingularResources = new FullObservableCollection<SingularResourceViewModel>();
        }

    }

}
