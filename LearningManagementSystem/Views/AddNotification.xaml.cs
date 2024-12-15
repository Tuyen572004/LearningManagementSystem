using CommunityToolkit.Mvvm.Messaging;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Enums;
using LearningManagementSystem.Messages;
using LearningManagementSystem.Services;
using LearningManagementSystem.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddNotification : Page
    {
        public NotificationViewModel NotificationViewModel { get; set; }

        private readonly IDao _dao = new SqlDao();
        private readonly UserService userService = new UserService();

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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NotificationViewModel = e.Parameter as NotificationViewModel;
            NotificationViewModel.Notification.PostDate = DateTime.Now;
            NotificationViewModel.Notification.ResourceCategoryId = (int)ResourceCategoryEnum.Notification;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            WeakReferenceMessenger.Default.Unregister<DialogMessage>(this);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private async Task ShowMessageDialog(string title, string content)
        {
            var messageDialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "Close",
                PrimaryButtonText = "Go Back",
                XamlRoot = this.Content.XamlRoot
            };

            var result = await messageDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                Frame.GoBack();
            }
            else
            {
                messageDialog.Hide();
            }
        }
    }
}
