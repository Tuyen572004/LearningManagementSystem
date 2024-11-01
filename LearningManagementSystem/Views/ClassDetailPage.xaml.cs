using LearningManagementSystem.ViewModels;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Controls;

namespace LearningManagementSystem.Views
{
    public sealed partial class ClassDetailPage : Page
    {
        public ClassDetailPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var classViewModel = e.Parameter as EnrollmentViewModel;
            DataContext = classViewModel;
        }
    }
}
