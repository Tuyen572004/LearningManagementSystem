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
    public sealed partial class EditClass : Page
    {
        /// <summary>
        /// Gets or sets the ViewModel for editing classes.
        /// </summary>
        public EditClassViewModel ViewModel { get; set; }

        /// <summary>
        /// Gets or sets the ViewModel for managing courses.
        /// </summary>
        private CourseViewModel CrsViewModel { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditClass"/> class.
        /// </summary>
        public EditClass()
        {
            this.InitializeComponent();
            ViewModel = new EditClassViewModel();
            CrsViewModel = new CourseViewModel();
            coursesComboBox.ItemsSource = CrsViewModel.Courses;
        }

        /// <summary>
        /// Invoked when the Page is loaded and becomes the current source of a parent Frame.
        /// </summary>
        /// <param name="e">Event data that can be examined by overriding code.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var oldTableClass = e.Parameter as TableClassesView;

            var oldClass = new Class
            {
                Id = oldTableClass.ID,
                ClassCode = oldTableClass.ClassCode,
                CourseId = oldTableClass.CourseID,
                ClassStartDate = oldTableClass.ClassStartDate,
                ClassEndDate = oldTableClass.ClassEndDate
            };

            ViewModel.SelectedClass = oldClass.Clone() as Class;

            inputClassCode.Text = ViewModel.SelectedClass.ClassCode;
            inputStartDate.Date = ViewModel.SelectedClass.ClassStartDate;
            inputEndDate.Date = ViewModel.SelectedClass.ClassEndDate;
            coursesComboBox.SelectedIndex = CrsViewModel.Courses.IndexOf(CrsViewModel.Courses.FirstOrDefault(x => x.Id == ViewModel.SelectedClass.CourseId));

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Handles the Click event of the cancel button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
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
                Frame.Navigate(typeof(ClassesPage), ViewModel.SelectedClass);
            }
            else
            {
                ctDialog.Hide();
            }
        }

        /// <summary>
        /// Handles the Click event of the save button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
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

            var dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Class",
                Content = "Are you sure you want to save changes?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No"
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var newClass = new Class
                {
                    Id = ViewModel.SelectedClass.Id,
                    ClassCode = inputClassCode.Text,
                    CourseId = (int)coursesComboBox.SelectedValue,
                    ClassStartDate = inputStartDate.Date.Value.DateTime,
                    ClassEndDate = inputEndDate.Date.Value.DateTime,
                };

                ViewModel.SelectedClass = newClass.Clone() as Class;
                ViewModel.UpdateClass(ViewModel.SelectedClass);
                Frame.Navigate(typeof(ClassesPage), ViewModel.SelectedClass);
            }
            else
            {
                dialog.Hide();
            }
        }
    }
}
