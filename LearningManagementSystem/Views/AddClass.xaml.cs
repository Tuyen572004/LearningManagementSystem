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
    public sealed partial class AddClass : Page
    {
        private ClassViewModel ViewModel { get; set; }

        private CourseViewModel CrsViewModel { get; set; }
        public AddClass()
        {
            this.InitializeComponent();
            ViewModel = new ClassViewModel();
            CrsViewModel = new CourseViewModel();
            coursesComboBox.ItemsSource = CrsViewModel.Courses;
        }

        private async void cancel_Click(object sender, RoutedEventArgs e)
        {
            var ctDialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Class",
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
            if (inputClassCode.Text.Length == 0)
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Class",
                    Content = "Class code is required.",
                    CloseButtonText = "Ok"
                }.ShowAsync();
                return;
            }

            if (coursesComboBox.SelectedValue == null)
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Class",
                    Content = "Course is required.",
                    CloseButtonText = "Ok"
                }.ShowAsync();
                return;
            }

            if (inputStartDate.Date > inputEndDate.Date)
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Class",
                    Content = "Start date must be before end date.",
                    CloseButtonText = "Ok"
                }.ShowAsync();
                return;
            }

            if (inputClassCode.Text.Length > 10)
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Class",
                    Content = "Class code must be less than 10 characters.",
                    CloseButtonText = "Ok"
                }.ShowAsync();
                return;
            }

            if (inputStartDate.Date == null)
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Class",
                    Content = "Start date is required.",
                    CloseButtonText = "Ok"
                }.ShowAsync();
                return;
            }

            if (inputEndDate.Date == null)
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Class",
                    Content = "End date is required.",
                    CloseButtonText = "Ok"
                }.ShowAsync();
                return;
            }

            var newClass = new Class
            {
                ClassCode = inputClassCode.Text,
                CourseId = (int)coursesComboBox.SelectedValue,
                ClassStartDate = inputStartDate.Date.Value.DateTime,
                ClassEndDate = inputEndDate.Date.Value.DateTime,
            };

            int count = ViewModel.InsertClass(newClass);

            await new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Class",
                Content = count == 1 ? "Class added successfully." : "Failed to add class.",
                CloseButtonText = "Ok"
            }.ShowAsync();

            Frame.GoBack();
        }
    }
}
