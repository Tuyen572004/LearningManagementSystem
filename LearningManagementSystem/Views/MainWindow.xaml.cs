using LearningManagementSystem.ViewModels;
using LearningManagementSystem.Views;
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

namespace LearningManagementSystem
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        //public MainWindow(EnrollmentClassesViewModel enrollmentClassesViewModel)
        //{
        //    this.InitializeComponent();
        //    // You can now use the enrollmentClassesViewModel as needed
        //    MainFrame.Navigate(typeof(Views.EnrollmentClassesPage), enrollmentClassesViewModel);
        //}

        public MainWindow()
        {
            Title = "Learning Management System";
            this.InitializeComponent();
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            content.Navigate(typeof(EnrollmentClassesPage));
        }
    }
}

