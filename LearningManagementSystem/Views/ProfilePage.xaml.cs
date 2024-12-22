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
        public ProfilePageViewModel ViewModel { get; set; }
        public ProfilePage()
        {
            this.InitializeComponent();
            OldPasswordInput.Visibility = Visibility.Collapsed;
            NewPasswordInput.Visibility = Visibility.Collapsed;
            ConfirmNewPasswordInput.Visibility = Visibility.Collapsed;
            SaveNewPassword.Visibility = Visibility.Collapsed;
            ViewModel = new ProfilePageViewModel();
        }
        private void ChangePasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangePasswordBtn.Visibility = Visibility.Collapsed;
            OldPasswordInput.Visibility = Visibility.Visible;
            NewPasswordInput.Visibility = Visibility.Visible;
            ConfirmNewPasswordInput.Visibility = Visibility.Visible;
            SaveNewPassword.Visibility = Visibility.Visible;
        }
        private void SaveNewPassword_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveChange_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
