using LearningManagementSystem.Command;
using LearningManagementSystem.Models;
using LearningManagementSystem.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Mysqlx.Prepare;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LearningManagementSystem.ViewModels
{
    public class EnrollmentViewModel : BaseViewModel
    {
        public Class Class { get; set; }

        public Course Course { get; set; }
        public ObservableCollection<Teacher> Teachers { get; set; }
        public Department Department { get; set; }

       // private INavigationService _navigationService;

       // public ICommand EnterCommand { get; }

        public string ClassTitle => $"{Course.CourseCode}-{Course.CourseDescription} {Class.ClassCode}";
        public string DepartmentName => $"Dept. of {Department.DepartmentCode}-{Department.DepartmentDesc}";

        public EnrollmentViewModel()
        {
            //_navigationService = DashBoard.NavigationService;
           // EnterCommand = new RelayCommand<EnrollmentViewModel>(CanExecute, EnterClassCommand);
        }


        //public bool CanExecute(object parameter)
        //{
        //    return true;
        //}

        //public void EnterClassCommand(object parameter)
        //{
        //    _navigationService.Navigate(typeof(ClassDetailPage), this);
        //}

    }
}
