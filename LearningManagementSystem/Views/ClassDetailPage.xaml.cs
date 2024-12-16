using CommunityToolkit.Mvvm.Messaging;
using LearningManagementSystem.Messages;
using LearningManagementSystem.Models;
using LearningManagementSystem.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Threading.Tasks;


namespace LearningManagementSystem.Views
{
    public sealed partial class ClassDetailPage : Page
    {
        public ClassDetailViewModel ClassDetailViewModel { get; set; }

        public ClassDetailPage()
        {
            this.InitializeComponent();

            ClassDetailViewModel = new ClassDetailViewModel();

            this.DataContext = this;

            // r : receiver, m : message
            WeakReferenceMessenger.Default.Register<NavigationMessage>(this, (r, m) =>
            {
                Frame.Navigate(m.DestinationPage, m.Parameter);
            });

            WeakReferenceMessenger.Default.Register<DialogMessage>(this, async (r, m) =>
{
    await ShowMessageDialog(m.Title, m.Value);
});

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is EnrollmentClassViewModel enrollmentViewModel) // navigated by EnrollmentClassesPage or AddNotification page
            {
                ClassDetailViewModel = new ClassDetailViewModel
                {
                    EnrollmentViewModel = enrollmentViewModel
                };
                // Eager loading
                ClassDetailViewModel.ResourceViewModel.LoadMoreItems(ClassDetailViewModel.EnrollmentViewModel.Class.Id);
            }
            //else if(e.Parameter is int classId) // navigated by Resources Page
            //{
            //    ClassDetailViewModel.EnrollmentViewModel.loadClassByClassId(classId);
            //    // Eager loading
            //    ClassDetailViewModel.ResourceViewModel.LoadMoreItems(ClassDetailViewModel.EnrollmentViewModel.Class.Id);
            //}

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            WeakReferenceMessenger.Default.Unregister<DialogMessage>(this);
            WeakReferenceMessenger.Default.Unregister<NavigationMessage>(this);
        }


        public void Title_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var textBlock = sender as TextBlock;
            var parameter = textBlock.DataContext as BaseResourceViewModel;
            var resource = parameter.BaseResource;
            if (resource != null)
            {
                if (resource is Assignment)
                {
                    var assignment = resource as Assignment;
                    Frame.Navigate(typeof(AssignmentPage), assignment);
                }
                else if (resource is Notification)
                {
                    var notification = resource as Notification;
                    Frame.Navigate(typeof(NotificationPage), notification);
                }
            }
        }

        // No Use Go Back: in case this page navigated by Resources Page
        // it will navigate to Resources Page instead of EnrollmentClassesPage
        public void BackButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EnrollmentClassesPage));
        }

        private ContentDialog _currentDialog;

        private async Task ShowMessageDialog(string title, string content)
        {
            try
            {
                _currentDialog = new ContentDialog
                {
                    Title = title,
                    Content = content,
                    CloseButtonText = "Close",
                    PrimaryButtonText = "Go Back",
                    XamlRoot = this.Content.XamlRoot // Ensure the dialog is shown in the root of the current view
                };

                var result = await _currentDialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    Frame.GoBack();
                }
                else
                {
                    _currentDialog?.Hide();
                }
            }
            catch (Exception ex)
            {
                _currentDialog?.Hide();
                // Optionally log the exception or show a different message
            }
            finally
            {
                _currentDialog = null;
            }
        }

    }
}

