using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using LearningManagementSystem.ViewModels;
using LearningManagementSystem.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditCourses : Page
    {
        /// <summary>
        /// Gets or sets the ViewModel for editing courses.
        /// </summary>
        public EditCourseViewModel ViewModel { get; set; }

        /// <summary>
        /// Gets or sets the ViewModel for managing departments.
        /// </summary>
        private DepartmentsViewModel DeViewModel { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditCourses"/> class.
        /// </summary>
        public EditCourses()
        {
            this.InitializeComponent();
            ViewModel = new EditCourseViewModel();
            DeViewModel = new DepartmentsViewModel();
            departmentComboBox.ItemsSource = DeViewModel.Departments;
        }

        /// <summary>
        /// Invoked when the Page is loaded and becomes the current source of a parent Frame.
        /// </summary>
        /// <param name="e">Event data that can be examined by overriding code. The parameter value is a NavigationEventArgs that contains data about the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var oldTableCourse = e.Parameter as TableCoursesView;

            var oldCourse = new Course
            {
                Id = oldTableCourse.ID,
                CourseCode = oldTableCourse.CourseCode,
                CourseDescription = oldTableCourse.CourseDecription,
                DepartmentId = oldTableCourse.DepartmentID
            };

            ViewModel.SelectedCourse = oldCourse.Clone() as Course;

            inputCourseCode.Text = ViewModel.SelectedCourse.CourseCode;
            inputCourseDescription.Text = ViewModel.SelectedCourse.CourseDescription;
            //DeViewModel.FindDepartmentID(ViewModel.SelectedCourse.DepartmentId.ToString());

            departmentComboBox.SelectedIndex = DeViewModel.Departments.IndexOf(DeViewModel.Departments.FirstOrDefault(x => x.Id == ViewModel.SelectedCourse.DepartmentId));

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Handles the Click event of the cancel button. Prompts the user to confirm cancellation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
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
                Frame.Navigate(typeof(CoursesPage), ViewModel.SelectedCourse);
            }
            else
            {
                ctDialog.Hide();
            }
        }

        /// <summary>
        /// Handles the Click event of the save button. Prompts the user to confirm saving changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private async void save_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Course",
                Content = "Are you sure you want to save changes?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No"
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var newCourse = new Course
                {
                    Id = ViewModel.SelectedCourse.Id,
                    CourseCode = inputCourseCode.Text,
                    CourseDescription = inputCourseDescription.Text,
                    DepartmentId = DeViewModel.FindDepartmentID(RvDepartment.Text)
                };

                ViewModel.SelectedCourse = newCourse.Clone() as Course;
                ViewModel.UpdateCourse(ViewModel.SelectedCourse);
                Frame.Navigate(typeof(CoursesPage), ViewModel.SelectedCourse);
            }
            else
            {
                dialog.Hide();
            }
        }
    }
}
