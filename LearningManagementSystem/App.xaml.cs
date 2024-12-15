using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;
using LearningManagementSystem.Services;
using LearningManagementSystem.DataAccess;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();
            this.InitializeComponent();
        }

        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ICloudinaryService, CloudinaryService>();
            services.AddSingleton<IDao, SqlDao>();

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {

            MainWindow = new LoginWindow();

            MainWindow.Activate();
        }

        public Window MainWindow;
    }
}
