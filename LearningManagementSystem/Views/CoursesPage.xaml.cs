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
using LearningManagementSystem.Helpers;
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

        public CourseViewModel CrsViewModel { get; set; }

        public CoursesPage()
        {
            this.InitializeComponent();
            ViewModel = new TableCoursesViewModel();
            CrsViewModel = new CourseViewModel();

            myCoursesTable.Visibility = Visibility.Collapsed;
            pagingNavi.Visibility = Visibility.Collapsed;
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
            pagingNavi.Visibility = Visibility.Visible;
            ViewModel.GetAllCourse();
            UpdatePagingInfo_bootstrap();
        }


        private void addCourses_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddCourse));
        }

        private async void deleteCourses_Click(object sender, RoutedEventArgs e)
        {
            if (myCoursesTable.SelectedItem is TableCoursesView selectedCourse)
            {
                var dialog = new ContentDialog()
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
                        DepartmentId = selectedCourse.DepartmentID
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
            if (e.Parameter != null)
            {
                var _oldData = e.Parameter as Course;
                var oldCourse = new TableCoursesView
                {
                    ID = _oldData.Id,
                    CourseCode = _oldData.CourseCode,
                    CourseDecription = _oldData.CourseDescription,
                    DepartmentID = _oldData.DepartmentId
                };

                ViewModel.SelectedCourse = oldCourse.Clone() as TableCoursesView;
            }

            base.OnNavigatedTo(e);
        }

        private void changeCourses_Click(object sender, RoutedEventArgs e)
        {
            if (myCoursesTable.SelectedItem is TableCoursesView selectedCourse)
            {
                ViewModel.SelectedCourse = selectedCourse.Clone() as TableCoursesView;


                Frame.Navigate(typeof(EditCourses), ViewModel.SelectedCourse);
            }
        }

        private void pagesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic item = pagesComboBox.SelectedItem;

            if (item != null)
            {
                ViewModel.Load(item.Page);
            }
        }


        public void UpdatePagingInfo_bootstrap()
        {
            var infoList = new List<object>();
            for (int i = 1; i <= ViewModel.TotalPages; i++)
            {
                infoList.Add(new
                {
                    Page = i,
                    Total = ViewModel.TotalPages
                });
            };

            pagesComboBox.ItemsSource = infoList;
            pagesComboBox.SelectedIndex = 0;



        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Load(1);
            UpdatePagingInfo_bootstrap();
        }

        private void previousBtn_Click(object sender, RoutedEventArgs e)
        {
            int i = pagesComboBox.SelectedIndex;
            if (i > 0)
            {
                pagesComboBox.SelectedIndex -= 1;
            }
        }

        // List of cats
        private List<string> Cats = new List<string>()
        {
            "Abyssinian",
            "Aegean",
            "American Bobtail"
        };

        // Handle text change and present suitable items
        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Since selecting an item will also change the text,
            // only listen to changes caused by user entering text.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var suitableItems = new List<string>();
                var splitText = sender.Text.ToLower().Split(" ");
                foreach (var cat in Cats)
                {
                    var found = splitText.All((key) =>
                    {
                        return cat.ToLower().Contains(key);
                    });
                    if (found)
                    {
                        suitableItems.Add(cat);
                    }
                }
                if (suitableItems.Count == 0)
                {
                    suitableItems.Add("No results found");
                }
                sender.ItemsSource = suitableItems;
            }
        }


        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            int i = pagesComboBox.SelectedIndex;
            if (i < pagesComboBox.Items.Count - 1)
            {
                pagesComboBox.SelectedIndex += 1;
            }
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {

        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {

        }

        private void sortOrder_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
