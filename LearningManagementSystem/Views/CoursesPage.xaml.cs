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
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CoursesPage : Page
    {
        private TableCoursesViewModel ViewModel { get; set; }

        /*
        void UpdatePagingInfo()
        {
            var infoList = new List<object>();
            for (int i = 1; i <= ViewModel.TotalPages; i++)
            {
                infoList.Add(new
                {
                    Page = i,
                    Total = ViewModel.TotalPages
                });
            }


        }
        */


        public CoursesPage()
        {
            this.InitializeComponent();
            ViewModel = new TableCoursesViewModel();
            StartRingProcess();
        }

        private async void StartRingProcess()
        {
            for (int i = 0; i <= 100; i++)
            {
                waitingRing.Value = i;
                await Task.Delay(50); // Adjust delay as needed
            }
            ViewModel.GetAllCourse();
        }


        private void rollGenerator_Click(object sender, RoutedEventArgs e)
        {
            //ViewModel.TableCourses.Add(new TableCoursesView
            //{
            //    ID = 123,
            //    CourseCode = "ABC",
            //    CourseDecription = "No Description",
            //    // = "IT",
            //    TotalStudents = 0,
            //    TotalSubjects = 0

            //});
            ViewModel.GetAllCourse();
        }

        private void addCourses_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.TableCourses.Add(new TableCoursesView
            {
                ID = 123,
                CourseCode = "ABC",
                CourseDecription = "No Description",
                // = "IT",
                TotalStudents = 0,
                TotalSubjects = 0

            });
        }
    }
}
