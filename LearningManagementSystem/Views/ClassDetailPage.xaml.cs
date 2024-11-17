using LearningManagementSystem.ViewModels;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Controls;
using LearningManagementSystem.Enums;
using LearningManagementSystem.Models;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Microsoft.UI.Xaml.Input;


namespace LearningManagementSystem.Views
{
    public sealed partial class ClassDetailPage : Page
    {
        public EnrollmentClassViewModel EnrollmentViewModel { get; set; }

        public ResourceViewModel ResourceViewModel { get; set; }

        public ClassDetailPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var classViewModel = e.Parameter as EnrollmentClassViewModel;
            DataContext = classViewModel;
            
            if(e.Parameter is EnrollmentClassViewModel enrollmentViewModel) // navigated by EnrollmentClassesPage (click Enter button)
            {
                EnrollmentViewModel = enrollmentViewModel;
                ResourceViewModel = new ResourceViewModel();
            }
            else if(e.Parameter is int classId) // navigated by Resources Page (click Back button)
            {
                ResourceViewModel = new ResourceViewModel();
                EnrollmentViewModel = new EnrollmentClassViewModel();
                EnrollmentViewModel.loadClassByClassId(classId);

            }
            base.OnNavigatedTo(e);

        }

        private void Expander_Expanding(Expander sender, ExpanderExpandingEventArgs args)
        {
            var category = (ResourceCategory)sender.Tag;
            ResourceViewModel.LoadMoreItems(category, EnrollmentViewModel.Class.Id);

        }

        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var textBlock = sender as TextBlock;
            var resource = textBlock.DataContext as BaseResource;
            if (resource != null)
            {
                if(resource is Assignment)
                {
                    var assignment = resource as Assignment;
                    Frame.Navigate(typeof(AssignmentPage), assignment);
                }
                // ------------ IMPLEMENT LATER ------------
                else if (resource is Notification)
                {
                    var notification = resource as Notification;
                    Frame.Navigate(typeof(NotificationPage), notification);
                }
                else if (resource is Document)
                {
                    var document = resource as Document;
                    Frame.Navigate(typeof(DocumentPage), document);
                }
            }
        }

        // IMPLEMENT LATER
        private void BackButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EnrollmentClassesPage));
        }
    }
}
