using LearningManagementSystem.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Windows.Input;

namespace LearningManagementSystem.Views
{
    public sealed partial class EnrollmentClassesPage : Page
    {
        private EnrollmentClassesViewModel ViewModel { get; set; }
        public EnrollmentClassesPage()
        {
            this.InitializeComponent();
            ViewModel = new EnrollmentClassesViewModel();
            ViewModel.LoadEnrolledClasses();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void NavigateToClassDetail(EnrollmentViewModel classViewModel)
        {
            Frame.Navigate(typeof(ClassDetailPage), classViewModel);
        }
    }

}
