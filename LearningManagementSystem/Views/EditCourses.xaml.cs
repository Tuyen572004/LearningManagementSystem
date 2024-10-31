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
        public EditCourseViewModel ViewModel { get; set; }

        private DepartmentsViewModel DeViewModel { get; set; }
        public EditCourses()
        {
            this.InitializeComponent();
            ViewModel = new EditCourseViewModel();
            DeViewModel = new DepartmentsViewModel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var oldCourse = e.Parameter as TableCoursesView;

            ViewModel.SelectedCourse = oldCourse.Clone() as TableCoursesView;

            base.OnNavigatedTo(e);
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
                Frame.Navigate(typeof(CoursesPage), ViewModel.SelectedCourse);
            }
            else
            {
                ctDialog.Hide();
            }

        }

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
                    CourseCode = inputCourseCode.Text,
                    CourseDescription = inputCourseDescription.Text,
                    DepartmentId = DeViewModel.FindDepartmentID(RvDepartment.Text)
                };

                ViewModel.SelectedCourse = newCourse.Clone() as TableCoursesView;
                Frame.Navigate(typeof(CoursesPage), ViewModel.SelectedCourse);
            }
            else
            {
                dialog.Hide();
            }
        }



    }
}
