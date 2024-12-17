using LearningManagementSystem.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace LearningManagementSystem.Views
{
    public sealed partial class EnrollmentClassesPage : Page
    {
        private EnrollmentClassesViewModel ViewModel { get; set; }
        public EnrollmentClassViewModel SelectedClass { get; set; }

        public EnrollmentClassesPage()
        {
            this.InitializeComponent();
            ViewModel = new EnrollmentClassesViewModel();
            ViewModel.LoadEnrolledClasses();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = new EnrollmentClassesViewModel();
            ViewModel.LoadEnrolledClasses();
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedClass = button?.CommandParameter as EnrollmentClassViewModel;
            if (selectedClass != null)
            {
                SelectedClass = selectedClass;
                // Handle the selected class
                Frame.Navigate(typeof(ClassDetailPage), selectedClass);
            }
        }

        private void addClass_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddClass));
        }

        private void deleteClass_Click(object sender, RoutedEventArgs e)
        {

        }

        private void editClass_Click(object sender, RoutedEventArgs e)
        {

        }
    }

}
