﻿using LearningManagementSystem.DataAccess;
using LearningManagementSystem.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        /// 

       // private ServiceProvider _serviceProvider;

        public App()
        {
            this.InitializeComponent();
 //           ConfigureServices();
        }

        //private void ConfigureServices()
        //{
        //    var serviceCollection = new ServiceCollection();

        //    // Register IDao with MockDao
        //    serviceCollection.AddSingleton<IDao, MockDao>();

        //    // Register MainWindow
        //    serviceCollection.AddSingleton<MainWindow>();

        //    // Register EnrollmentClassesViewModel
        //    serviceCollection.AddTransient<EnrollmentClassesViewModel>();

        //    // Build the service provider
        //    _serviceProvider = serviceCollection.BuildServiceProvider();
        //}

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new DashBoard();
            m_window.Activate();
        }

        private Window m_window;
    }
}
