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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginOfTeacherPage : Page
    {
        public LoginWindowViewModel ViewModel { get; set; }
        public LoginOfTeacherPage()
        {
            this.InitializeComponent();
            ViewModel = new LoginWindowViewModel();
        }

        private string EncryptPassword(string password)
        {
            var passwordRaw = password;
            var passwordInBytes = Encoding.UTF8.GetBytes(passwordRaw);

            var encryptedPasswordBase64 = Convert.ToBase64String(passwordInBytes);

            return encryptedPasswordBase64;
        }
        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            var username = inputUsername.Text;
            var password = inputPassword.Text;

            var passwordhash = EncryptPassword(password);
            ViewModel.UserLogin = new User
            {
                Username = username,
                PasswordHash = passwordhash.Length > 4096 ? passwordhash[..4096] : passwordhash,
                Role = "teacher"
            };

            if (ViewModel.CheckUserInfo())
            {
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


    }
}
