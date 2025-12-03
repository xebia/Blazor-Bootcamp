using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ClaimTracker.Data;

namespace ClaimTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize database on application startup
            // This creates the database and seeds data on first run
            try
            {
                DatabaseManager.InitializeDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to initialize database:\n\n{ex.Message}", 
                    "Database Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                
                // Exit the application if database initialization fails
                Current.Shutdown();
            }
        }
    }
}
