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
using LearningManagementSystem.Models;
using LearningManagementSystem.ViewModels;
using LearningManagementSystem.Helpers;
using System.Collections.ObjectModel;
using LearningManagementSystem.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddAccount : Page
    {
        private UserViewModel ViewModel { get; set; }

        private UserService MyUserService { get; set; }
        public ObservableCollection<string> Roles { get; set; }

        public AddAccount()
        {
            this.InitializeComponent();
            ViewModel = new UserViewModel();
            Roles = new ObservableCollection<string>
            {
                "admin",
                "student",
                "teacher"
            };
            MyUserService = new UserService();
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
                Frame.GoBack();
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
                var rawPassword = inputPassword.Text;

                var user = new User
                {
                    Username = inputUsername.Text,
                    PasswordHash = MyUserService.EncryptPassword(rawPassword),
                    Email = inputEmail.Text,
                    Role = roleComboBox.SelectedValue.ToString(),
                    CreatedAt = DateTime.Now
                };

                int count = ViewModel.InsertUser(user);

                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "User",
                    Content = count == 1 ? "User added successfully" : "Failed to add user",
                    CloseButtonText = "Ok"
                }.ShowAsync();

                Frame.GoBack();
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
