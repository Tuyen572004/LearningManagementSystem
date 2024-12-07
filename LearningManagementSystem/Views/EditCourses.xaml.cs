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
            departmentComboBox.ItemsSource = DeViewModel.Departments;
        }

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
            if (RvDepartment.Text != "" && inputCourseCode.Text != "")
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
            else
            {
                if (RvDepartment.Text == "")
                {
                    InfoBar.Message = "Please choose a department.";
                }
                else if (inputCourseCode.Text == "")
                {
                    InfoBar.Message = "Please fill Cources Code.";
                }

                InfoBar.IsOpen = true;
                await System.Threading.Tasks.Task.Delay(10000);
                InfoBar.IsOpen = false;
            }
        }



    }
}
