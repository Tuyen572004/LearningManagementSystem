using LearningManagementSystem.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace LearningManagementSystem.Views
{
    public sealed partial class EnrollmentClassesPage : Page
    {
        /// <summary>
        /// Gets or sets the ViewModel for the EnrollmentClassesPage.
        /// </summary>
        private EnrollmentClassesViewModel ViewModel { get; set; }

        /// <summary>
        /// Gets or sets the selected class.
        /// </summary>
        public EnrollmentClassViewModel SelectedClass { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnrollmentClassesPage"/> class.
        /// </summary>
        public EnrollmentClassesPage()
        {
            this.InitializeComponent();
            ViewModel = new EnrollmentClassesViewModel();
            ViewModel.LoadEnrolledClasses();
        }

        /// <summary>
        /// Handles the Loaded event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = new EnrollmentClassesViewModel();
            ViewModel.LoadEnrolledClasses();
        }

        /// <summary>
        /// Handles the Click event of the EnterButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// Handles the Click event of the addClass control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void addClass_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddClass));
        }

        /// <summary>
        /// Handles the Click event of the deleteClass control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void deleteClass_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Handles the Click event of the editClass control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void editClass_Click(object sender, RoutedEventArgs e)
        {

        }
    }

}
