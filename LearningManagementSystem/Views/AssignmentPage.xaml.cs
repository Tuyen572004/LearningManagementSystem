using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.UI.Controls;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Messages;
using LearningManagementSystem.Models;
using LearningManagementSystem.Services;
using LearningManagementSystem.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

            WeakReferenceMessenger.Default.Register<BusyMessage>(this, (r, m) =>
            {
                AssignmentViewModel.IsBusy = m.Value;
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) // navigated by ClassDetailPage (click into 1 assignment to see it in detail)
        {
            AssignmentViewModel.Assignment = e.Parameter as Assignment;
            AssignmentViewModel.LoadSubmissions();
            base.OnNavigatedTo(e);
        }

        private async void  BackButton_Click(object sender, RoutedEventArgs e)
        {
            //this.Frame.Navigate(typeof(ClassDetailPage), AssignmentViewModel.Assignment.ClassId);

            if (AssignmentViewModel.IsEditing)
            {
                var dialog = new ContentDialog
                {
                    Title = "Confirm",
                    Content = "You have unsaved changes. Do you really want to leave?",
                    PrimaryButtonText = "Yes",
                    CloseButtonText = "No"
                };

                var result = await dialog.ShowAsync();

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
        

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            WeakReferenceMessenger.Default.Unregister<DialogMessage>(this);
            WeakReferenceMessenger.Default.Unregister<BusyMessage>(this);
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

        private void dataGridTeacherView_Sorting(object sender, CommunityToolkit.WinUI.UI.Controls.DataGridColumnEventArgs e)
        {
            if(e.Column.Tag.ToString() == "StudentCode")
            {
                if(e.Column.SortDirection==null||e.Column.SortDirection==DataGridSortDirection.Ascending) {
                    AssignmentViewModel.Submissions.OrderBy(submission => submission.Student.StudentCode);
                }
                else
                {
                    AssignmentViewModel.Submissions.OrderByDescending(submission => submission.Student.StudentCode);
                }
            }
            else if (e.Column.Tag.ToString() == "SubmissionDate")
            {
                if(e.Column.SortDirection == null|| e.Column.SortDirection == DataGridSortDirection.Ascending) {
                    AssignmentViewModel.Submissions.OrderBy(submission => submission.Submission.SubmissionDate);
                }
                else
                {
                    AssignmentViewModel.Submissions.OrderByDescending(submission => submission.Submission.SubmissionDate);
                }
            }
            else if (e.Column.Tag.ToString() == "Grade")
            {
                if (e.Column.SortDirection == null|| e.Column.SortDirection == DataGridSortDirection.Ascending) {
                    AssignmentViewModel.Submissions.OrderBy(submission => submission.Submission.Grade);
                }
                else
                {
                    AssignmentViewModel.Submissions.OrderByDescending(submission => submission.Submission.Grade);
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            dataGridTeacherView.ItemsSource = new ObservableCollection<SubmissionViewModel>(AssignmentViewModel.Submissions.Where(submission => submission.Student.StudentCode.Contains(searchBox.Text)));
        }
    }
}
