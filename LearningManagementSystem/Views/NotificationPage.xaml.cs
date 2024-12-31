using CommunityToolkit.Mvvm.Messaging;
using LearningManagementSystem.Messages;
using LearningManagementSystem.Models;
using LearningManagementSystem.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Threading.Tasks;

namespace LearningManagementSystem.Views
{
    public sealed partial class NotificationPage : Page
    {
        /// <summary>
        /// Gets or sets the NotificationViewModel.
        /// </summary>
        public NotificationViewModel NotificationViewModel { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationPage"/> class.
        /// </summary>
        public NotificationPage()
        {
            this.InitializeComponent();
            NotificationViewModel = new NotificationViewModel();
            this.DataContext = NotificationViewModel;

            WeakReferenceMessenger.Default.Register<DialogMessage>(this, async (r, m) =>
            {
                await ShowMessageDialog(m.Title, m.Value);
            });

            WeakReferenceMessenger.Default.Register<BusyMessage>(this, (r, m) =>
            {
                NotificationViewModel.IsBusy = m.Value;
            });
        }

        /// <summary>
        /// Called when the page is navigated to.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NotificationViewModel.Notification = e.Parameter as Notification;
            base.OnNavigatedTo(e);
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
                    Frame.GoBack();
                }
                else
                {
                    _currentDialog.Hide();
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

        /// <summary>
        /// Handles the Back button click event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (NotificationViewModel.IsEditing)
                {
                    var _currentDialog = new ContentDialog
                    {
                        Title = "Confirm",
                        Content = "You have unsaved changes. Do you really want to leave?",
                        PrimaryButtonText = "Yes",
                        CloseButtonText = "No",
                        XamlRoot = this.Content.XamlRoot // Ensure the dialog is shown in the root of the current view
                    };

                    var result = await _currentDialog.ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {
                        Frame.GoBack();
                    }
                }
                else
                {
                    Frame.GoBack();
                }
            }
            catch (Exception ex)
            {
                _currentDialog?.Hide();
                // Optionally log the exception or show a different message
            }
        }

        /// <summary>
        /// Called when the page is navigated from.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            WeakReferenceMessenger.Default.Unregister<DialogMessage>(this);
            WeakReferenceMessenger.Default.Unregister<BusyMessage>(this);
            base.OnNavigatedFrom(e);
        }
    }
}
