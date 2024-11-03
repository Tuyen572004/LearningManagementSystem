using LearningManagementSystem.ViewModels;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Controls;
using LearningManagementSystem.Enums;
using LearningManagementSystem.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace LearningManagementSystem.Views
{
    public sealed partial class ClassDetailPage : Page
    {

        public EnrollmentViewModel EnrollmentViewModel { get; set; }

        public ResourceViewModel ResourceViewModel { get; set; }

        public ClassDetailPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            EnrollmentViewModel = e.Parameter as EnrollmentViewModel;
            ResourceViewModel = new ResourceViewModel();
            
        }

        private void Expander_Expanding(Expander sender, ExpanderExpandingEventArgs args)
        {
            var category = (ResourceCategory)sender.Tag;
            ResourceViewModel.LoadMoreItems(category, EnrollmentViewModel.Class.Id);

        }
    }
}
