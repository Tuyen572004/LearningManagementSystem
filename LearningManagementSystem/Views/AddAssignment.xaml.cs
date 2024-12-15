using CommunityToolkit.Mvvm.Messaging;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Enums;
using LearningManagementSystem.Messages;
using LearningManagementSystem.Services;
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
    public sealed partial class AddAssignment : Page
    {

        public AssignmentViewModel AssignmentViewModel { get; set; }

        private readonly IDao _dao = App.Current.Services.GetService<IDao>();
        private readonly UserService userService = new UserService();
        public AddAssignment()
        {
            this.InitializeComponent();
            AssignmentViewModel = new AssignmentViewModel();
            AssignmentViewModel.Assignment.ResourceCategoryId = (int)ResourceCategoryEnum.Assignment;

            WeakReferenceMessenger.Default.Register<DialogMessage>(this, async (r, m) =>
            {
                await ShowMessageDialog(m.Title, m.Value);
            });
        }

       protected override void OnNavigatedTo(NavigationEventArgs e) // navigated by ClassDetailPage (click into Add Assignment button)
        {
            AssignmentViewModel = e.Parameter as AssignmentViewModel;
            AssignmentViewModel.Assignment.DueDate = DateTime.Now;
            AssignmentViewModel.Assignment.ResourceCategoryId = (int)ResourceCategoryEnum.Assignment;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            WeakReferenceMessenger.Default.Unregister<DialogMessage>(this);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            //this.Frame.Navigate(typeof(ClassDetailPage), AssignmentViewModel.Assignment.ClassId);
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
