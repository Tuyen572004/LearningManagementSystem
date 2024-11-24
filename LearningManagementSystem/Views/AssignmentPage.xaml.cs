using CommunityToolkit.Mvvm.Messaging;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Messages;
using LearningManagementSystem.Models;
using LearningManagementSystem.Services;
using LearningManagementSystem.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AssignmentPage : Page
    {

        public AssignmentViewModel AssignmentViewModel { get; set; }

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
            this.Frame.Navigate(typeof(ClassDetailPage), AssignmentViewModel.Assignment.ClassId);
        }



        private async Task ShowMessageDialog(string title, string content)
        {
            var messageDialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot // Ensure the dialog is shown in the root of the current view
            };

            await messageDialog.ShowAsync();
        }



    }
}
