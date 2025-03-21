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
using LearningManagementSystem.Services.UserService;
using System.Text.RegularExpressions;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddAccount : Page
    {
        /// <summary>
        /// Gets or sets the ViewModel for managing user-related operations and data.
        /// </summary>
        private UserViewModel ViewModel { get; set; }

        /// <summary>
        /// Gets or sets the UserService for handling user-related services.
        /// </summary>
        private UserService MyUserService { get; set; }

        /// <summary>
        /// Gets or sets the collection of roles available for selection.
        /// </summary>
        public ObservableCollection<string> Roles { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddAccount"/> class.
        /// </summary>
        public AddAccount()
        {
            this.InitializeComponent();
            ViewModel = new UserViewModel();
            MyUserService = new UserService();
            Roles = new ObservableCollection<string>
                {
                    "admin",
                    "student",
                    "teacher"
                };
        }

        /// <summary>
        /// Handles the click event of the cancel button.
        /// Displays a confirmation dialog and navigates back if confirmed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
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

        /// <summary>
        /// Handles the click event of the save button.
        /// Validates the input, creates a new user, and displays the result.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private async void save_Click(object sender, RoutedEventArgs e)
        {
            var rawPassword = "12345";

            var user = new User
            {
                Username = inputUsername.Text,
                PasswordHash = MyUserService.EncryptPassword(rawPassword),
                Email = inputEmail.Text,
                Role = roleComboBox.SelectedValue.ToString(),
                CreatedAt = DateTime.Now
            };

            if (string.IsNullOrEmpty(inputUsername.Text))
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "User",
                    Content = "Username cannot be empty",
                    CloseButtonText = "Ok"
                }.ShowAsync();
                return;
            }

            Regex emailRegex = new(@"^\d{8}@student\.hcmus\.edu\.vn$|^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (!emailRegex.IsMatch(user.Email))
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "User",
                    Content = "Invalid email",
                    CloseButtonText = "Ok"
                }.ShowAsync();
                return;
            }

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
    }
}
