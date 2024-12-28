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
                var newUser = new User
                {
                    Username = inputUsername.Text,
                    Email = inputEmail.Text,
                    PasswordHash = ViewModel.resetPassword ? ViewModel.MyUserService.EncryptPassword("12345") : ViewModel.SelectedUser.PasswordHash,
                    Role = ViewModel.SelectedUser.Role,
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

        private async void resetPasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Reset Password",
                Content = "Are you sure you want to reset the password?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No"
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                ViewModel.resetPassword = true;
            }

            var dialog2 = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Reset Password",
                Content = "Password has been reset.",
                CloseButtonText = "Ok"
            };

            await dialog2.ShowAsync();
        }
    }
}
