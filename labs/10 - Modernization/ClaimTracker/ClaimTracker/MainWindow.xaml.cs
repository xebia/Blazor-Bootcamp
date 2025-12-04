using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClaimTracker.Windows;

namespace ClaimTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// This demonstrates traditional WPF navigation patterns using code-behind.
    /// Common in legacy applications - pages are loaded directly into content areas.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads a page/user control into the main content area.
        /// Traditional WPF pattern for page navigation.
        /// </summary>
        private void LoadPage(UserControl page)
        {
            ContentArea.Child = page;
        }

        /// <summary>
        /// Menu handler: File > Exit
        /// </summary>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            // Prompt for confirmation before closing
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to exit?",
                "Exit Application",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Menu handler: Claims > View All Claims
        /// Loads the Claims List page.
        /// </summary>
        private void ViewClaims_Click(object sender, RoutedEventArgs e)
        {
            LoadPage(new ClaimsListPage());
            StatusText.Text = "Claims Management";
        }

        /// <summary>
        /// Legacy menu handler for backward compatibility.
        /// Maps old "Edit > Claims" to new "Claims > View All Claims"
        /// </summary>
        private void EditClaims_Click(object sender, RoutedEventArgs e)
        {
            ViewClaims_Click(sender, e);
        }

        /// <summary>
        /// Menu handler: Claims > Add New Claim
        /// Opens the Add Claim dialog directly from the menu.
        /// </summary>
        private void AddNewClaim_Click(object sender, RoutedEventArgs e)
        {
            // Traditional pattern: Open modal dialog from menu
            var addWindow = new AddEditClaimWindow();
            addWindow.Owner = this;
            
            bool? result = addWindow.ShowDialog();
            
            // If a claim was added, navigate to the claims page to show it
            if (result == true)
            {
                LoadPage(new ClaimsListPage());
                StatusText.Text = "Claims Management";
            }
        }
    }
}
