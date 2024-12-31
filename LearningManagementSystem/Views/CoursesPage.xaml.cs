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
        /// <summary>
        /// Gets or sets the ViewModel for managing table courses.
        /// </summary>
        public TableCoursesViewModel ViewModel { get; set; }

        /// <summary>
        /// Gets or sets the ViewModel for managing courses.
        /// </summary>
        public CourseViewModel CrsViewModel { get; set; }

        /// <summary>
        /// Gets or sets the collection of options for sorting courses.
        /// </summary>
        public ObservableCollection<string> sortByOptions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoursesPage"/> class.
        /// </summary>
        public CoursesPage()
        {
            this.InitializeComponent();
            ViewModel = new TableCoursesViewModel();
            CrsViewModel = new CourseViewModel();
            sortByOptions = new ObservableCollection<string>
                {
                    "Default",
                    "ID",
                    "Course Code",
                    "Course Description",
                    "Department ID",
                };
            myCoursesTable.Visibility = Visibility.Collapsed;
            pagingNavi.Visibility = Visibility.Collapsed;
            SearchBar.Visibility = Visibility.Collapsed;
            SortPanel.Visibility = Visibility.Collapsed;
            StartRingProcess();
        }

        /// <summary>
        /// Starts the ring process animation and loads the courses.
        /// </summary>
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
            SearchBar.Visibility = Visibility.Visible;
            SortPanel.Visibility = Visibility.Visible;
            ViewModel.GetAllCourse();
            UpdatePagingInfo_bootstrap();
        }

        /// <summary>
        /// Navigates to the AddCourse page.
        /// </summary>
        private void addCourses_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddCourse));
        }

        /// <summary>
        /// Deletes the selected course.
        /// </summary>
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
                    var check = ViewModel.RemoveCourse(new Course
                    {
                        Id = selectedCourse.ID,
                        CourseCode = selectedCourse.CourseCode,
                        CourseDescription = selectedCourse.CourseDecription,
                        DepartmentId = selectedCourse.DepartmentID
                    });
                    if (check == 0)
                    {
                        myCoursesTable.SelectedItem = null;
                        ViewModel.TableCourses.Remove(selectedCourse);
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
                        await new ContentDialog()
                        {
                            XamlRoot = this.XamlRoot,
                            Content = "Course cannot be removed",
                            Title = "Error",
                            CloseButtonText = "Ok"
                        }.ShowAsync();
                    }
                }
                else
                {
                    dialog.Hide();
                }
            }
        }

        /// <summary>
        /// Handles the navigation to the page and sets the selected course if provided.
        /// </summary>
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

        /// <summary>
        /// Navigates to the EditCourses page with the selected course.
        /// </summary>
        private void changeCourses_Click(object sender, RoutedEventArgs e)
        {
            if (myCoursesTable.SelectedItem is TableCoursesView selectedCourse)
            {
                ViewModel.SelectedCourse = selectedCourse.Clone() as TableCoursesView;
                Frame.Navigate(typeof(EditCourses), ViewModel.SelectedCourse);
            }
        }

        /// <summary>
        /// Handles the selection change in the pages combo box and loads the selected page.
        /// </summary>
        private void pagesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic item = pagesComboBox.SelectedItem;

            if (item != null)
            {
                ViewModel.Load(item.Page);
            }
        }

        /// <summary>
        /// Updates the paging information in the combo box.
        /// </summary>
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

        /// <summary>
        /// Handles the click event for the previous button and navigates to the previous page.
        /// </summary>
        private void previousBtn_Click(object sender, RoutedEventArgs e)
        {
            int i = pagesComboBox.SelectedIndex;
            if (i > 0)
            {
                pagesComboBox.SelectedIndex -= 1;
            }
        }

        /// <summary>
        /// Handles the text changed event for the AutoSuggestBox and provides suggestions.
        /// </summary>
        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var suitableItems = new List<string>();
                var splitText = sender.Text.ToLower().Split(" ");
                if (ViewModel.Suggestion != null)
                {
                    foreach (var keyword in ViewModel.Suggestion)
                    {
                        var found = splitText.All((key) =>
                        {
                            return keyword.ToLower().Contains(key);
                        });
                        if (found)
                        {
                            suitableItems.Add(keyword);
                        }
                    }
                    if (suitableItems.Count == 0)
                    {
                        suitableItems.Add("No results found");
                    }
                    sender.ItemsSource = suitableItems;
                }
            }
        }

        /// <summary>
        /// Handles the click event for the next button and navigates to the next page.
        /// </summary>
        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            int i = pagesComboBox.SelectedIndex;
            if (i < pagesComboBox.Items.Count - 1)
            {
                pagesComboBox.SelectedIndex += 1;
            }
        }

        /// <summary>
        /// Handles the query submitted event for the AutoSuggestBox and performs a search.
        /// </summary>
        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var searchText = args.QueryText;
            ViewModel.Keyword = searchText;
            ViewModel.Load();
            UpdatePagingInfo_bootstrap();
        }

        /// <summary>
        /// Handles the suggestion chosen event for the AutoSuggestBox and performs a search.
        /// </summary>
        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var searchText = args.SelectedItem.ToString();
            ViewModel.Keyword = searchText;
            ViewModel.Load();
            UpdatePagingInfo_bootstrap();
        }

        /// <summary>
        /// Handles the click event for the sort order button and updates the sort order.
        /// </summary>
        private void sortOrder_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.countRepeatButton++;
            var modi = ViewModel.countRepeatButton % 3;

            if (modi == 0)
                sortOrder.Content = "ASC";
            else if (modi == 1)
                sortOrder.Content = "DESC";
            else
                sortOrder.Content = "Default";

            if (sortOrder.Content.ToString() == "Default")
            {
                ViewModel.SortOrder = "ASC";
            }
            else
                ViewModel.SortOrder = sortOrder.Content.ToString();

            ViewModel.Load();
            UpdatePagingInfo_bootstrap();
        }

        /// <summary>
        /// Handles the selection change event for the sort by combo box and updates the sort field.
        /// </summary>
        private void sortByComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SortBy = sortByOptions[sortByComboBox.SelectedIndex].Replace(" ", "");
            if (ViewModel.SortBy == "Default")
                ViewModel.SortBy = "Id";

            ViewModel.Load();
            UpdatePagingInfo_bootstrap();
        }
    }
}
