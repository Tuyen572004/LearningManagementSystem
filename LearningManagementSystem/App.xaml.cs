﻿using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Services;
using LearningManagementSystem.Services.CloudinaryService;
using LearningManagementSystem.Services.ConfigService;
using LearningManagementSystem.Services.EmailService;
using LearningManagementSystem.Services.UserService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using System;
using MathNet.Numerics;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object. This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Services = ConfigureServices();
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the current application instance.
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <summary>
        /// Gets the service provider instance.
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Configures the services for dependency injection.
        /// </summary>
        /// <returns>The service provider.</returns>
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IConfigService, ConfigFileService>();
            // Register SqlDao
            services.AddSingleton<IDao, SqlDao>();

            // Register CloudinaryService
            services.AddSingleton<ICloudinaryService, CloudinaryService>();

            // Register BrevoEmailService
            services.AddSingleton<IEmailService, BrevoEmailService>();

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user. Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            MainWindow = new LoginWindow();
            MainWindow.Activate();

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(MainWindow);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.SetPresenter(AppWindowPresenterKind.Default);
            appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 1280, Height = 720 });

            MainWindow.Title = "Learning Management System";

            base.OnLaunched(args);
        }

        /// <summary>
        /// Gets or sets the main window of the application.
        /// </summary>
        public Microsoft.UI.Xaml.Window MainWindow;
    }
}
