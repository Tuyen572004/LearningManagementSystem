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
        public NotificationViewModel NotificationViewModel { get; set; }

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

        protected override void OnNavigatedTo(NavigationEventArgs e) // navigated by ClassDetailPage (click into 1 assignment to see it in detail)
        {
            NotificationViewModel.Notification = e.Parameter as Notification;
            base.OnNavigatedTo(e);
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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            WeakReferenceMessenger.Default.Unregister<DialogMessage>(this);
            WeakReferenceMessenger.Default.Unregister<BusyMessage>(this);
            base.OnNavigatedFrom(e);
        }
    }
}
