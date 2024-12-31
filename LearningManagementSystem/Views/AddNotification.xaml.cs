using CommunityToolkit.Mvvm.Messaging;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Enums;
using LearningManagementSystem.Messages;
using LearningManagementSystem.Services.UserService;
using LearningManagementSystem.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddNotification : Page
    {
        /// <summary>
        /// Gets or sets the NotificationViewModel.
        /// </summary>
        public NotificationViewModel NotificationViewModel { get; set; }

        private readonly IDao _dao = App.Current.Services.GetService<IDao>();
        private readonly UserService userService = new UserService();

        /// <summary>
        /// Initializes a new instance of the <see cref="AddNotification"/> class.
        /// </summary>
        public AddNotification()
        {
            this.InitializeComponent();
            NotificationViewModel = new NotificationViewModel();
            NotificationViewModel.Notification.ResourceCategoryId = (int)ResourceCategoryEnum.Notification;

            WeakReferenceMessenger.Default.Register<DialogMessage>(this, async (r, m) =>
            {
                await ShowMessageDialog(m.Title, m.Value);
            });
        }

        /// <summary>
        /// Invoked when the Page is loaded and becomes the current source of a parent Frame.
        /// </summary>
        /// <param name="e">Event data that can be examined by overriding code. The event data is representative of the pending navigation that will load the current Page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NotificationViewModel = e.Parameter as NotificationViewModel;
            NotificationViewModel.Notification.PostDate = DateTime.Now;
            NotificationViewModel.Notification.ResourceCategoryId = (int)ResourceCategoryEnum.Notification;
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Invoked immediately before the Page is unloaded and is no longer the current source of a parent Frame.
        /// </summary>
        /// <param name="e">Event data that can be examined by overriding code. The event data is representative of the pending navigation that will unload the current Page.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            WeakReferenceMessenger.Default.Unregister<DialogMessage>(this);
        }

        /// <summary>
        /// Handles the Back button click event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data that can be examined by overriding code.</param>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
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
    }
}
