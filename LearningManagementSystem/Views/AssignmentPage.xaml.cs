using CommunityToolkit.Mvvm.Messaging;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Messages;
using LearningManagementSystem.Models;
using LearningManagementSystem.Services;
using LearningManagementSystem.ViewModels;
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
    public sealed partial class AssignmentPage : Page
    {

        public AssignmentViewModel AssignmentViewModel {get; set; }

        private readonly IDao _dao = new SqlDao();
        private readonly UserService userService = new UserService();
        public AssignmentPage()
        {
            this.InitializeComponent();
            AssignmentViewModel = new AssignmentViewModel();
            this.DataContext = AssignmentViewModel;

            WeakReferenceMessenger.Default.Register<DialogMessage>(this, async (r, m) =>
            {
                await ShowMessageDialog(m.Title, m.Value);
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) // navigated by ClassDetailPage (click into 1 assignment to see it in detail)
        {
            AssignmentViewModel.Assignment = e.Parameter as Assignment;
            AssignmentViewModel.LoadSubmissions();
            base.OnNavigatedTo(e);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            //this.Frame.Navigate(typeof(ClassDetailPage), AssignmentViewModel.Assignment.ClassId);
            Frame.GoBack();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            WeakReferenceMessenger.Default.Unregister<DialogMessage>(this);
            base.OnNavigatedFrom(e);
        }




        private async Task ShowMessageDialog(string title, string content)
        {
            var messageDialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "Close",
                PrimaryButtonText = "Go Back",
                XamlRoot = this.Content.XamlRoot // Ensure the dialog is shown in the root of the current view
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
