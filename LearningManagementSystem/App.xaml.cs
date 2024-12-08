
using Microsoft.UI.Xaml;
using Npgsql;
using System;
using System.IO;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {

            // MainWindow = new LoginWindow();
            //var Host = "dpg-ct3vdm52ng1s73a13mr0-a"; // Extracted hostname
            //var Username = "posgre";
            //var Password = "sOL87JmxSGdTuGCwXDNNc8ehNbyctMVH"; 
            //var Database = "lmsdb_7mch";

            //var connectionString = $"Host={Host};Username={Username};Password={Password};Database={Database}";

            //using var connection = new NpgsqlConnection(connectionString); 
            //connection.Open();

            //var Host = "dpg-ct3vdm52ng1s73a13mr0-a"; // The hostname of your PostgreSQL server
            //var Username = "posgre";
            //var Password = "sOL87JmxSGdTuGCwXDNNc8ehNbyctMVH";
            //var Database = "lmsdb_7mch";

            //var connectionString = $"Host={Host};Username={Username};Password={Password};Database={Database}";

            //var connectionString = "Host=dpg-ct3vdm52ng1s73a13mr0-a.singapore-postgres.render.com;Username=posgre;Password=sOL87JmxSGdTuGCwXDNNc8ehNbyctMVH;Database=lmsdb_7mch";
            //var connection = new NpgsqlConnection(connectionString);
            //connection.Open();

            //Console.WriteLine("Connected to the database!");


            MainWindow = new LoginWindow();

            // MainWindow = new DashBoard();
            MainWindow.Activate();
        }

        public Window MainWindow;
    }
}
