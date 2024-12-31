using LearningManagementSystem.Services.UserService;
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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProfilePage : Page
    {
        /// <summary>
        /// Gets or sets the ViewModel for the ProfilePage.
        /// </summary>
        public ProfilePageViewModel ViewModel { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfilePage"/> class.
        /// </summary>
        public ProfilePage()
        {
            this.InitializeComponent();
            OldPasswordInput.Visibility = Visibility.Collapsed;
            NewPasswordInput.Visibility = Visibility.Collapsed;
            ConfirmNewPasswordInput.Visibility = Visibility.Collapsed;
            SaveNewPassword.Visibility = Visibility.Collapsed;
            Cancel.Visibility = Visibility.Collapsed;
            ViewModel = new ProfilePageViewModel();
            UserNameInput.Text = ViewModel.UserName;
            EmailInput.Text = ViewModel.Email;
        }

        /// <summary>
        /// Handles the Click event of the ChangePasswordBtn control.
        /// Shows the password change inputs and hides the ChangePassword button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ChangePasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangePasswordBtn.Visibility = Visibility.Collapsed;
            OldPasswordInput.Visibility = Visibility.Visible;
            NewPasswordInput.Visibility = Visibility.Visible;
            ConfirmNewPasswordInput.Visibility = Visibility.Visible;
            SaveNewPassword.Visibility = Visibility.Visible;
            Cancel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Restarts the application.
        /// </summary>
        public static void RestartApp()
        {
            var loginWindow = new LoginWindow();
            loginWindow.Activate();
            var dashboardWindow = (Application.Current as App)?.MainWindow;
            dashboardWindow?.Close();
            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName,
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(processInfo);
            Application.Current.Exit();
        }

        /// <summary>
        /// Handles the Click event of the SaveNewPassword control.
        /// Validates and saves the new password.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void SaveNewPassword_Click(object sender, RoutedEventArgs e)
        {
            var oldPassword = OldPasswordInput.Password;
            var newPassword = NewPasswordInput.Password;
            var confirmNewPassword = ConfirmNewPasswordInput.Password;

            if (newPassword.Length == 0)
            {
                var dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Password Required",
                    Content = "Please enter a new password.",
                    CloseButtonText = "Ok"
                };
                await dialog.ShowAsync();
                return;
            }

            if (newPassword != confirmNewPassword)
            {
                var dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Password Mismatch",
                    Content = "New password and confirm new password do not match. Please try again.",
                    CloseButtonText = "Ok"
                };
                await dialog.ShowAsync();
                return;
            }
            else if (ViewModel.MyUserService.EncryptPassword(oldPassword) != ViewModel.Password)
            {
                var dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Incorrect Password",
                    Content = "The old password you entered is incorrect. Please try again.",
                    CloseButtonText = "Ok"
                };
                await dialog.ShowAsync();
            }
            else
            {
                ViewModel.Password = ViewModel.MyUserService.EncryptPassword(newPassword);
                ViewModel.UpdateProfile();

                var dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Password Updated",
                    Content = "Your password has been updated successfully. Please sign in again.",
                    CloseButtonText = "Ok"
                };
                await dialog.ShowAsync();

                RestartApp();
            }
        }

        /// <summary>
        /// Handles the Click event of the SaveChange control.
        /// Validates and saves the profile changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void SaveChange_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(UserNameInput.Text))
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Username Required",
                    Content = "Please enter a username.",
                    CloseButtonText = "Ok"
                }.ShowAsync();
                return;
            }

            if (string.IsNullOrEmpty(EmailInput.Text))
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Email Required",
                    Content = "Please enter an email address.",
                    CloseButtonText = "Ok"
                }.ShowAsync();
                return;
            }

            Regex emailRegex = new(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (!emailRegex.IsMatch(EmailInput.Text))
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "User",
                    Content = "Invalid email.",
                    CloseButtonText = "Ok"
                }.ShowAsync();
                return;
            }

            ViewModel.UserName = UserNameInput.Text;
            ViewModel.Email = EmailInput.Text;
            ViewModel.UpdateProfile();

            var dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Profile Updated",
                Content = "Your profile has been updated successfully. Please sign in again.",
                CloseButtonText = "Ok"
            };

            await dialog.ShowAsync();

            RestartApp();
        }

        /// <summary>
        /// Handles the Click event of the Cancel control.
        /// Hides the password change inputs and shows the ChangePassword button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            OldPasswordInput.Visibility = Visibility.Collapsed;
            NewPasswordInput.Visibility = Visibility.Collapsed;
            ConfirmNewPasswordInput.Visibility = Visibility.Collapsed;
            SaveNewPassword.Visibility = Visibility.Collapsed;
            Cancel.Visibility = Visibility.Collapsed;
            ChangePasswordBtn.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Handles the Click event of the EditUserInput control.
        /// Enables editing of the username input.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void EditUserInput_Click(object sender, RoutedEventArgs e)
        {
            UserNameInput.IsReadOnly = false;
            UserNameInput.Focus(FocusState.Programmatic);
        }

        /// <summary>
        /// Handles the Click event of the EditEmailInput control.
        /// Enables editing of the email input.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void EditEmailInput_Click(object sender, RoutedEventArgs e)
        {
            EmailInput.IsReadOnly = false;
            EmailInput.Focus(FocusState.Programmatic);
        }
    }
}
