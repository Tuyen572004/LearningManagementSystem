using CommunityToolkit.WinUI.UI.Controls;
using LearningManagementSystem.Controls;
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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ClassesPage : Page
    {
        public TableClassesViewModel ViewModel { get; set; }

        public ClassViewModel ClsViewModel { get; set; }

        public ObservableCollection<string> sortByOptions { get; set; }

        public ClassesPage()
        {
            this.InitializeComponent();
            ViewModel = new TableClassesViewModel();
            ClsViewModel = new ClassViewModel();
            sortByOptions = new ObservableCollection<string>
            {
                "Default",
                "ID",
                "Class Code",
                "Course ID",
                "Class Start Date",
                "Class End Date"

            };
            myClassesTable.Visibility = Visibility.Collapsed;
            pagingNavi.Visibility = Visibility.Collapsed;
            SearchBar.Visibility = Visibility.Collapsed;
            SortPanel.Visibility = Visibility.Collapsed;
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
            myClassesTable.Visibility = Visibility.Visible;
            pagingNavi.Visibility = Visibility.Visible;
            SearchBar.Visibility = Visibility.Visible;
            SortPanel.Visibility = Visibility.Visible;
            ViewModel.GetAllClass();
            UpdatePagingInfo_bootstrap();
        }


        private void addClass_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddClass));
        }

        private async void deleteClass_Click(object sender, RoutedEventArgs e)
        {
            if (myClassesTable.SelectedItem is TableClassesView selectedClass)
            {
                var dialog = new ContentDialog()
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Delete Class",
                    Content = "Are you sure you want to delete this class?",
                    PrimaryButtonText = "Yes",
                    CloseButtonText = "No"
                };

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    ViewModel.RemoveClass(new Class
                    {
                        Id = selectedClass.ID,
                        ClassCode = selectedClass.ClassCode,
                        CourseId = selectedClass.CourseID,
                        ClassStartDate = selectedClass.ClassStartDate,
                        ClassEndDate = selectedClass.ClassEndDate
                    });
                    myClassesTable.SelectedItem = null;
                    ViewModel.TableClasses.Remove(selectedClass);
                    await new ContentDialog()
                    {
                        XamlRoot = this.XamlRoot,
                        Content = "Class removed",
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
                var _oldData = e.Parameter as Class;
                var oldClass = new TableClassesView
                {
                    ID = _oldData.Id,
                    ClassCode = _oldData.ClassCode,
                    CourseID = _oldData.CourseId,
                    ClassStartDate = _oldData.ClassStartDate,
                    ClassEndDate = _oldData.ClassEndDate
                };

                ViewModel.SelectedClass = oldClass.Clone() as TableClassesView;
            }

            base.OnNavigatedTo(e);
        }

        private void changeClass_Click(object sender, RoutedEventArgs e)
        {
            if (myClassesTable.SelectedItem is TableClassesView selectedClass)
            {
                ViewModel.SelectedClass = selectedClass.Clone() as TableClassesView;
                Frame.Navigate(typeof(EditClass), ViewModel.SelectedClass);
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
        private void previousBtn_Click(object sender, RoutedEventArgs e)
        {
            int i = pagesComboBox.SelectedIndex;
            if (i > 0)
            {
                pagesComboBox.SelectedIndex -= 1;
            }
        }
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
            var searchText = args.QueryText;
            ViewModel.Keyword = searchText;
            ViewModel.Load();
            UpdatePagingInfo_bootstrap();
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var searchText = args.SelectedItem.ToString();
            ViewModel.Keyword = searchText;
            ViewModel.Load();
            UpdatePagingInfo_bootstrap();
        }

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

        private void sortByComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SortBy = sortByOptions[sortByComboBox.SelectedIndex].Replace(" ", "");
            if (ViewModel.SortBy == "Default")
                ViewModel.SortBy = "Id";

            ViewModel.Load();
            UpdatePagingInfo_bootstrap();
        }

        // Thuc's part, please don't delete :DD
        private void myClassesTable_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (myClassesTable.SelectedItem is TableClassesView selectedClass)
            {
                var selectedClassId = selectedClass.ID;
                Frame.Navigate(typeof(ClassInfoWithParticipant), selectedClassId);
            }
        }
    }
}
