using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using ClaimTracker.Models;
using ClaimTracker.Windows;

namespace ClaimTracker.Pages
{
    /// <summary>
    /// Displays the list of all claims with add, edit, and delete functionality.
    /// This demonstrates traditional WPF code-behind data access patterns - common in legacy applications.
    /// </summary>
    public partial class ClaimsListPage : UserControl
    {
        private readonly string connectionString = 
            ConfigurationManager.ConnectionStrings["ClaimsDB"].ConnectionString;

        public ClaimsListPage()
        {
            InitializeComponent();
            LoadClaims();
        }

        /// <summary>
        /// Loads all claims from the database and binds to the DataGrid.
        /// Traditional ADO.NET approach typical of legacy WPF apps.
        /// </summary>
        private void LoadClaims()
        {
            try
            {
                DataTable claimsTable = new DataTable();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT ClaimId, ClaimNumber, PolicyHolderName, DateOfIncident, 
                               DateFiled, ClaimAmount, Status, Description
                        FROM Claims
                        ORDER BY DateFiled DESC";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.Fill(claimsTable);
                }

                ClaimsDataGrid.ItemsSource = claimsTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading claims: {ex.Message}",
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Event handler for Add New Claim button.
        /// Opens the AddEditClaimWindow in Add mode.
        /// </summary>
        private void AddNewClaim_Click(object sender, RoutedEventArgs e)
        {
            // Traditional WPF pattern: Open modal dialog window
            var addWindow = new AddEditClaimWindow();
            addWindow.Owner = Window.GetWindow(this);
            
            bool? result = addWindow.ShowDialog();
            
            // Refresh grid if claim was added
            if (result == true)
            {
                LoadClaims();
            }
        }

        /// <summary>
        /// Event handler for Edit button in the DataGrid.
        /// Opens the AddEditClaimWindow in Edit mode.
        /// </summary>
        private void EditClaim_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn?.Tag != null)
            {
                int claimId = (int)btn.Tag;
                
                // Traditional WPF pattern: Open modal dialog window with claim ID
                var editWindow = new AddEditClaimWindow(claimId);
                editWindow.Owner = Window.GetWindow(this);
                
                bool? result = editWindow.ShowDialog();
                
                // Refresh grid if claim was updated
                if (result == true)
                {
                    LoadClaims();
                }
            }
        }

        /// <summary>
        /// Event handler for Delete button in the DataGrid.
        /// Shows confirmation dialog before deleting.
        /// </summary>
        private void DeleteClaim_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn?.Tag == null) return;

            int claimId = (int)btn.Tag;

            // Get the claim details for the confirmation message
            DataRowView row = ClaimsDataGrid.SelectedItem as DataRowView;
            if (row == null) return;

            string claimNumber = row["ClaimNumber"].ToString();
            string policyHolder = row["PolicyHolderName"].ToString();

            // Show confirmation dialog
            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete this claim?\n\n" +
                $"Claim Number: {claimNumber}\n" +
                $"Policy Holder: {policyHolder}",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                DeleteClaim(claimId);
            }
        }

        /// <summary>
        /// Deletes a claim from the database.
        /// </summary>
        private void DeleteClaim(int claimId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string deleteSql = "DELETE FROM Claims WHERE ClaimId = @ClaimId";
                    
                    using (SqlCommand command = new SqlCommand(deleteSql, connection))
                    {
                        command.Parameters.AddWithValue("@ClaimId", claimId);
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Claim deleted successfully.",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Refresh the grid
                LoadClaims();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error deleting claim: {ex.Message}",
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
