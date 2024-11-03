using LearningManagementSystem.Helper;
using LearningManagementSystem.Models;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    public class AssignmentViewModel : BaseViewModel
    {
        public Assignment Assignment { get; set; }

        public string DueStatus => IsDue ? "Closed" : "On Going";

        public string DueDate
        {
            get
            {
                return Assignment.DueDate.ToString("dd/MM/yyyy, HH:mm");
            }
            set
            {
                Assignment.DueDate = DateTime.Parse(value);
            }
        }

        public bool IsDue
        {
            get
            {
                return Assignment.DueDate < DateTime.Now;
            }
        }


        public void UpdateDueStatus()
        {
            OnPropertyChanged(nameof(DueStatus));
        }

    }
}
