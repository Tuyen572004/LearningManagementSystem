using LearningManagementSystem.Models;
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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginForm : Page
    {
        /// <summary>
        /// Gets or sets the ViewModel for the login window.
        /// </summary>
        public LoginWindowViewModel ViewModel { get; set; }

        /// <summary>
        /// Gets or sets the user service for handling user-related operations.
        /// </summary>
        public UserService MyUserService { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginForm"/> class.
        /// </summary>
        public LoginForm()
        {
            this.InitializeComponent();
            MyUserService = new UserService();
            ViewModel = new LoginWindowViewModel();
        }

        /// <summary>
        /// Handles the click event of the login button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            var username = inputUsername.Text;
            var password = inputPassword.Password;

            var passwordhash = MyUserService.EncryptPassword(password);
            ViewModel.UserLogin = new User
            {
                Username = username,
                PasswordHash = passwordhash.Length > 4096 ? passwordhash[..4096] : passwordhash,
            };
            try
            {
                if (ViewModel.CheckUserInfo())
                {
                    ViewModel.GetUser();
                    UserService.SaveUserConfig(ViewModel.UserLogin);
                    var screen = new DashBoard();
                    screen.Activate();
                }
                else
                {
                    var ctDialog = new ContentDialog
                    {
                        XamlRoot = this.XamlRoot,
                        Title = "Oops !",
                        Content = "Incorrect Username or Password",
                        CloseButtonText = "Try Again"
                    };
                    var result = await ctDialog.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                var ctDialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Error",
                    Content = $"An error occurred: {ex.Message}\nPlease Try Again",
                    CloseButtonText = "Try Again"
                };
                var result = await ctDialog.ShowAsync();
            }
        }

        /// <summary>
        /// Handles the checked event of the reveal mode checkbox.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void revealModeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            inputPassword.PasswordRevealMode = PasswordRevealMode.Visible;
        }

        /// <summary>
        /// Handles the unchecked event of the reveal mode checkbox.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void revealModeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            inputPassword.PasswordRevealMode = PasswordRevealMode.Hidden;
        }
    }
}
