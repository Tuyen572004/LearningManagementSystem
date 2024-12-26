using LearningManagementSystem.Models;
using LearningManagementSystem.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddCourse : Page
    {
        private CourseViewModel ViewModel { get; set; }

        private DepartmentsViewModel DeViewModel { get; set; }
        public AddCourse()
        {
            this.InitializeComponent();
            ViewModel = new CourseViewModel();
            DeViewModel = new DepartmentsViewModel();
            departmentComboBox.ItemsSource = DeViewModel.Departments;
        }

        private async void cancel_Click(object sender, RoutedEventArgs e)
        {
            var ctDialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Course",
                Content = "Are you sure you want to cancel?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No"
            };


            var result = await ctDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                Frame.GoBack();
            }
            else
            {
                ctDialog.Hide();
            }

        }

        private async void save_Click(object sender, RoutedEventArgs e)
        {
            var course = new Course
            {
                CourseCode = inputCourseCode.Text,
                CourseDescription = inputCourseDescription.Text,
                DepartmentId = DeViewModel.FindDepartmentID(RvDepartment.Text)
            };

            if (course.CourseCode.Length == 0)
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Course",
                    Content = "Course code is required.",
                    CloseButtonText = "Ok"
                }.ShowAsync();
                return;
            }

            if (course.CourseCode.Length > 10)
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Course",
                    Content = "Course code must be less than 10 characters.",
                    CloseButtonText = "Ok"
                }.ShowAsync();
                return;
            }

            if (course.CourseDescription.Length > 100)
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Course",
                    Content = "Course description must be less than 100 characters.",
                    CloseButtonText = "Ok"
                }.ShowAsync();
                return;
            }

            if (course.DepartmentId == 0)
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Course",
                    Content = "Department is required.",
                    CloseButtonText = "Ok"
                }.ShowAsync();
                return;
            }

            int count = ViewModel.InsertCourse(course);

            await new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Course",
                Content = count == 1 ? "Course added successfully." : "Failed to add course.",
                CloseButtonText = "Ok"
            }.ShowAsync();

            Frame.GoBack();
        }
    }
}
