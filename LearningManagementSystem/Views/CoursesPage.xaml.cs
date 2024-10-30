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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using LearningManagementSystem.Helper;
using LearningManagementSystem.DataAccess;
using System.Threading.Tasks;
using LearningManagementSystem.Views;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem
{
    public sealed partial class CoursesPage : Page
    {
        public TableCoursesViewModel ViewModel { get; set; }

        public CoursesPage()
        {
            this.InitializeComponent();
            ViewModel = new TableCoursesViewModel();
            myCoursesTable.Visibility = Visibility.Collapsed;
            StartRingProcess();
        }

        private async void StartRingProcess()
        {
            for (int i = 0; i <= 100; i++)
            {
                waitingRing.Value = i;
                await Task.Delay(15);
                if (i == 100)
                {
                    waitingRing.Visibility = Visibility.Collapsed;
                }
            }
            myCoursesTable.Visibility = Visibility.Visible;
            ViewModel.GetAllCourse();
        }


        private void addCourses_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddCourse));
        }

        private async void deleteCourses_Click(object sender, RoutedEventArgs e)
        {
            if (myCoursesTable.SelectedItem is TableCoursesView selectedCourse)
            {
                var dialog=new ContentDialog()
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Delete Course",
                    Content = "Are you sure you want to delete this course?",
                    PrimaryButtonText = "Yes",
                    CloseButtonText = "No"
                };

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    ViewModel.RemoveCourse(new Course
                    {
                        Id = selectedCourse.ID,
                        CourseCode = selectedCourse.CourseCode,
                        CourseDescription = selectedCourse.CourseDecription,
                        DepartmentId = 0
                    });
                    myCoursesTable.SelectedItem = null; // Clear the selection after deletion

                    ViewModel.TableCourses.Remove(selectedCourse);


                    // Feedback
                    await new ContentDialog()
                    {
                        XamlRoot = this.XamlRoot,
                        Content = "Course removed",
                        Title = "Success",
                        CloseButtonText = "Ok"
                    }.ShowAsync();
                }
                else
                {
                    dialog.Hide();
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter!=null)
            {
                var _oldData = e.Parameter as TableCoursesView;
                ViewModel.SelectedCourse = _oldData;
            }

            base.OnNavigatedTo(e);
        }

        private void changeCourses_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EditCourses), ViewModel.SelectedCourse);
        }
    }
}
