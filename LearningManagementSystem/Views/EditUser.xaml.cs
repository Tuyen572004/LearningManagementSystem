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
using System.Collections.ObjectModel;
using LearningManagementSystem.Models;
using LearningManagementSystem.ViewModels;
using LearningManagementSystem.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditUser : Page
    {
        public EditUserViewModel ViewModel { get; set; }
        public ObservableCollection<string> Roles { get; set; }

        public UserService MyUserService { get; set; }
        public EditUser()
        {
            this.InitializeComponent();
            Roles = new ObservableCollection<string>
            {
                "admin",
                "student",
                "teacher"
            };
            ViewModel = new EditUserViewModel();
            MyUserService = new UserService();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var oldTableUser = e.Parameter as TableUsersView;

            var oldUser = new User
            {
                Username = oldTableUser.Username,
                PasswordHash = oldTableUser.PasswordHash,
                Role = oldTableUser.Role,
                Email = oldTableUser.Email,
                Id = oldTableUser.ID,
                CreatedAt = oldTableUser.CreatedAt,
            };

            ViewModel.SelectedUser = oldUser.Clone() as User;

            inputEmail.Text = ViewModel.SelectedUser.Email;
            inputPassword.Text = ViewModel.SelectedUser.PasswordHash;
            roleComboBox.SelectedIndex = Roles.IndexOf(ViewModel.SelectedUser.Role);
            inputUsername.Text = ViewModel.SelectedUser.Username;

            base.OnNavigatedTo(e);
        }

        private async void cancel_Click(object sender, RoutedEventArgs e)
        {
            var ctDialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "User",
                Content = "Are you sure you want to cancel?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No"
            };

            var result = await ctDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                Frame.Navigate(typeof(AdminPage), ViewModel.SelectedUser);
            }
            else
            {
                ctDialog.Hide();
            }

        }

        private async void save_Click(object sender, RoutedEventArgs e)
        {
            if (inputUsername.Text != "" && inputPassword.Text != "" && roleComboBox.SelectedValue.ToString() != "" && inputEmail.Text != "")
            {
                var dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "User",
                    Content = "Are you sure you want to save changes?",
                    PrimaryButtonText = "Yes",
                    CloseButtonText = "No"
                };

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    var rawPassword = inputPassword.Text;
                    var newUser = new User
                    {
                        Username = inputUsername.Text,
                        PasswordHash = MyUserService.EncryptPassword(rawPassword),
                        Email = inputEmail.Text,
                        Role = roleComboBox.SelectedValue.ToString(),
                        Id = ViewModel.SelectedUser.Id,
                    };

                    ViewModel.SelectedUser = newUser.Clone() as User;
                    ViewModel.UpdateUser(ViewModel.SelectedUser);
                    Frame.Navigate(typeof(AdminPage), ViewModel.SelectedUser);
                }
                else
                {
                    dialog.Hide();
                }
            }
            else
            {
                if (inputUsername.Text == "")
                {
                    InfoBar.Message = "Username is required.";
                }
                else if (inputPassword.Text == "")
                {
                    InfoBar.Message = "Password is required.";
                }
                else if (roleComboBox.SelectedValue.ToString() == "")
                {
                    InfoBar.Message = "Role is required.";
                }
                else if (inputEmail.Text == "")
                {
                    InfoBar.Message = "Email is required.";
                }
                InfoBar.IsOpen = true;
                await System.Threading.Tasks.Task.Delay(10000);
                InfoBar.IsOpen = false;
            }
        }
    }
}
