using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using LearningManagementSystem.ViewModels;

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
