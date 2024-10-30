using LearningManagementSystem.Models;
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
    public sealed partial class AddCourse : Page
    {
        private CourseViewModel ViewModel { get; set; }
        public AddCourse()
        {
            this.InitializeComponent();
            ViewModel = new CourseViewModel();
        }

        private async void cancel_Click(object sender, RoutedEventArgs e)
        {
            var ctDialog = new ContentDialog
            {
                XamlRoot= this.XamlRoot,
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
                DepartmentId = int.Parse(inputDepartmentID.Text)
            };

            int count=ViewModel.InsertCourse(course);

            await new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Course",
                Content = count == 1 ? "Course added successfully" : "Failed to add course",
                CloseButtonText = "Ok"
            }.ShowAsync();

            Frame.GoBack();
        }

        private void departmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic item = departmentComboBox.SelectedItem;

            if (item != null)
            {
                inputDepartmentID.Text = item.Id.ToString();
            }

        }
    }
}
