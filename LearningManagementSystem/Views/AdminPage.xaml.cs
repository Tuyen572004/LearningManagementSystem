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
using System.Threading.Tasks;
using LearningManagementSystem.ViewModels;
using LearningManagementSystem.Models;
using System.Collections.ObjectModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdminPage : Page
    {
        public TableUsersViewModel ViewModel { get; set; }

        public UserViewModel UsrViewModel { get; set; }

        public ObservableCollection<string> sortByOptions { get; set; }
        public AdminPage()
        {
            this.InitializeComponent();
            ViewModel = new TableUsersViewModel();
            UsrViewModel = new UserViewModel();
            sortByOptions = new ObservableCollection<string>
            {
                "Default",
                "ID",
                "Username",
                "Email",
                "Role",
                "Created At"
            };

            pagingNavi.Visibility = Visibility.Collapsed;
            sortPanel.Visibility = Visibility.Collapsed;
            searchBar.Visibility = Visibility.Collapsed;
            myUsersTable.Visibility = Visibility.Collapsed;
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
            pagingNavi.Visibility = Visibility.Visible;
            sortPanel.Visibility = Visibility.Visible;
            searchBar.Visibility = Visibility.Visible;
            myUsersTable.Visibility = Visibility.Visible;
            ViewModel.GetAllUser();
            UpdatePagingInfo_bootstrap();
        }



        private async void deleteAccount_Click(object sender, RoutedEventArgs e)
        {
            if (myUsersTable.SelectedItem is TableUsersView selectedUser)
            {
                var dialog = new ContentDialog()
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Delete User",
                    Content = "Are you sure you want to delete this user?",
                    PrimaryButtonText = "Yes",
                    CloseButtonText = "No"
                };

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    ViewModel.RemoveUser(new User
                    {
                        Id = selectedUser.ID,
                        Username = selectedUser.Username,
                        PasswordHash = selectedUser.PasswordHash,
                        Email = selectedUser.Email,
                        Role = selectedUser.Role,
                        CreatedAt = selectedUser.CreatedAt
                    });
                    myUsersTable.SelectedItem = null; // Clear the selection after deletion

                    ViewModel.TableUsers.Remove(selectedUser);


                    // Feedback
                    await new ContentDialog()
                    {
                        XamlRoot = this.XamlRoot,
                        Content = "User is removed",
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
                var _oldData = e.Parameter as User;
                var oldUser = new TableUsersView
                {
                    ID = _oldData.Id,
                    Username = _oldData.Username,
                    PasswordHash = _oldData.PasswordHash,
                    Email = _oldData.Email,
                    Role = _oldData.Role,
                    CreatedAt = _oldData.CreatedAt

                };

                ViewModel.SelectedUser = oldUser.Clone() as TableUsersView;
            }

            base.OnNavigatedTo(e);
        }
        private void changeAccount_Click(object sender, RoutedEventArgs e)
        {
            if (myUsersTable.SelectedItem is TableUsersView selectedUser)
            {
                ViewModel.SelectedUser = selectedUser.Clone() as TableUsersView;


                Frame.Navigate(typeof(EditUser), ViewModel.SelectedUser);
            }
        }

        private void addAccount_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddAccount));
        }
        private void previousBtn_Click(object sender, RoutedEventArgs e)
        {
            int i = pagesComboBox.SelectedIndex;
            if (i > 0)
            {
                pagesComboBox.SelectedIndex -= 1;
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

        private void sortByComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SortBy = sortByOptions[sortByComboBox.SelectedIndex].Replace(" ", "");
            if (ViewModel.SortBy == "Default")
                ViewModel.SortBy = "Id";

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
    }
}
