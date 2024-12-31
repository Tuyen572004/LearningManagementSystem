using CommunityToolkit.Mvvm.Messaging;
using LearningManagementSystem.Enums;
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
    /// <summary>
    /// Represents the detail page for a class.
    /// </summary>
    public sealed partial class ClassDetailPage : Page
    {
        /// <summary>
        /// Gets or sets the view model for the class detail page.
        /// </summary>
        public ClassDetailViewModel ClassDetailViewModel { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassDetailPage"/> class.
        /// </summary>
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

            WeakReferenceMessenger.Default.Register<DeleteResourceMessage>(this, async (r, m) =>
            {
                await ConfirmAndDeleteResource(m.Value as BaseResourceViewModel);
            });
        }

        /// <summary>
        /// Confirms and deletes the specified resource.
        /// </summary>
        /// <param name="resource">The resource to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task ConfirmAndDeleteResource(BaseResourceViewModel resource)
        {
            var confirmDialog = new ContentDialog
            {
                Title = "Confirm Delete",
                Content = "Are you sure you want to delete this resource?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                XamlRoot = this.Content.XamlRoot // Ensure the dialog is shown in the root of the current view
            };

            var result = await confirmDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                await DeleteResource(resource);
            }
        }

        /// <summary>
        /// Deletes the specified resource.
        /// </summary>
        /// <param name="resource">The resource to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task DeleteResource(BaseResourceViewModel resource)
        {
            await ClassDetailViewModel.ResourceViewModel.DeleteResource(resource);
        }

        /// <summary>
        /// Called when the page is navigated to.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is EnrollmentClassViewModel enrollmentViewModel) // navigated by EnrollmentClassesPage or AddNotification page
            {
                ClassDetailViewModel.EnrollmentViewModel = enrollmentViewModel;
                // Eager loading
                ClassDetailViewModel.ResourceViewModel.LoadMoreItems(ClassDetailViewModel.EnrollmentViewModel.Class.Id);
            }
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Called when the page is navigated from.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        /// <summary>
        /// Handles the double-tap event on the title.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
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

        /// <summary>
        /// Handles the click event of the back button.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        public void BackButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private ContentDialog _currentDialog;

        /// <summary>
        /// Shows a message dialog with the specified title and content.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="content">The content of the dialog.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
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
                    if (Frame.CanGoBack)
                    {
                        Frame.GoBack();
                    }
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

