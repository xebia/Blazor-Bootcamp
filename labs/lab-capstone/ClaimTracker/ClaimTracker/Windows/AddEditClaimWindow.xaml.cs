using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Windows;
using ClaimTracker.Models;

namespace ClaimTracker.Windows
{
    /// <summary>
    /// Modal dialog for adding or editing a claim.
    /// Demonstrates traditional WPF code-behind validation patterns - common in legacy applications.
    /// </summary>
    public partial class AddEditClaimWindow : Window
    {
        private readonly string connectionString = 
            ConfigurationManager.ConnectionStrings["ClaimsDB"].ConnectionString;

        private readonly int? claimId; // Null for Add mode, populated for Edit mode
        private readonly bool isEditMode;

        /// <summary>
        /// Constructor for Add mode - creates a new claim.
        /// </summary>
        public AddEditClaimWindow()
        {
            InitializeComponent();
            isEditMode = false;
            WindowTitle.Text = "Add New Claim";
            this.Title = "Add New Claim";
            
            // Set default values
            DateFiledPicker.SelectedDate = DateTime.Today;
            StatusComboBox.SelectedIndex = 0; // Default to "Pending"
        }

        /// <summary>
        /// Constructor for Edit mode - loads and edits an existing claim.
        /// Traditional WPF pattern: Constructor overload to determine mode.
        /// </summary>
        public AddEditClaimWindow(int claimId) : this()
        {
            this.claimId = claimId;
            isEditMode = true;
            WindowTitle.Text = "Edit Claim";
            this.Title = "Edit Claim";
            
            LoadClaim();
        }

        /// <summary>
        /// Loads claim data from the database for editing.
        /// Traditional ADO.NET approach typical of legacy apps.
        /// </summary>
        private void LoadClaim()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT ClaimNumber, PolicyHolderName, DateOfIncident, DateFiled, 
                               ClaimAmount, Status, Description
                        FROM Claims
                        WHERE ClaimId = @ClaimId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ClaimId", claimId.Value);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Populate form fields - direct data binding to controls (legacy pattern)
                                ClaimNumberTextBox.Text = reader["ClaimNumber"].ToString();
                                PolicyHolderNameTextBox.Text = reader["PolicyHolderName"].ToString();
                                DateOfIncidentPicker.SelectedDate = (DateTime)reader["DateOfIncident"];
                                DateFiledPicker.SelectedDate = (DateTime)reader["DateFiled"];
                                ClaimAmountTextBox.Text = ((decimal)reader["ClaimAmount"]).ToString("F2");
                                
                                string status = reader["Status"].ToString();
                                foreach (System.Windows.Controls.ComboBoxItem item in StatusComboBox.Items)
                                {
                                    if (item.Content.ToString() == status)
                                    {
                                        StatusComboBox.SelectedItem = item;
                                        break;
                                    }
                                }

                                DescriptionTextBox.Text = reader["Description"]?.ToString() ?? "";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading claim: {ex.Message}",
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                
                this.DialogResult = false;
                this.Close();
            }
        }

        /// <summary>
        /// Validates all form fields.
        /// Traditional code-behind validation pattern - typical of legacy WPF apps.
        /// </summary>
        private bool ValidateClaim()
        {
            StringBuilder errors = new StringBuilder();

            // Required field: Claim Number
            if (string.IsNullOrWhiteSpace(ClaimNumberTextBox.Text))
            {
                errors.AppendLine("• Claim Number is required.");
            }
            else if (ClaimNumberTextBox.Text.Length > 50)
            {
                errors.AppendLine("• Claim Number cannot exceed 50 characters.");
            }

            // Required field: Policy Holder Name
            if (string.IsNullOrWhiteSpace(PolicyHolderNameTextBox.Text))
            {
                errors.AppendLine("• Policy Holder Name is required.");
            }
            else if (PolicyHolderNameTextBox.Text.Length > 100)
            {
                errors.AppendLine("• Policy Holder Name cannot exceed 100 characters.");
            }

            // Required field: Date of Incident (must be in the past)
            if (!DateOfIncidentPicker.SelectedDate.HasValue)
            {
                errors.AppendLine("• Date of Incident is required.");
            }
            else if (DateOfIncidentPicker.SelectedDate.Value.Date >= DateTime.Today)
            {
                errors.AppendLine("• Date of Incident must be in the past (not today or future).");
            }

            // Required field: Date Filed
            if (!DateFiledPicker.SelectedDate.HasValue)
            {
                errors.AppendLine("• Date Filed is required.");
            }

            // Required field: Claim Amount (must be > 0)
            if (string.IsNullOrWhiteSpace(ClaimAmountTextBox.Text))
            {
                errors.AppendLine("• Claim Amount is required.");
            }
            else if (!decimal.TryParse(ClaimAmountTextBox.Text, out decimal amount))
            {
                errors.AppendLine("• Claim Amount must be a valid number.");
            }
            else if (amount <= 0)
            {
                errors.AppendLine("• Claim Amount must be greater than 0.");
            }

            // Required field: Status
            if (StatusComboBox.SelectedItem == null)
            {
                errors.AppendLine("• Status is required.");
            }

            // Optional field: Description (max length only)
            if (!string.IsNullOrWhiteSpace(DescriptionTextBox.Text) && 
                DescriptionTextBox.Text.Length > 500)
            {
                errors.AppendLine("• Description cannot exceed 500 characters.");
            }

            // Display errors if any
            if (errors.Length > 0)
            {
                ValidationErrorText.Text = errors.ToString();
                ValidationErrorPanel.Visibility = Visibility.Visible;
                return false;
            }

            // Hide error panel if validation passes
            ValidationErrorPanel.Visibility = Visibility.Collapsed;
            return true;
        }

        /// <summary>
        /// Save button click handler.
        /// Validates and saves the claim to the database.
        /// </summary>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Validate form
            if (!ValidateClaim())
            {
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql;
                    if (isEditMode)
                    {
                        // UPDATE existing claim
                        sql = @"
                            UPDATE Claims 
                            SET ClaimNumber = @ClaimNumber,
                                PolicyHolderName = @PolicyHolderName,
                                DateOfIncident = @DateOfIncident,
                                DateFiled = @DateFiled,
                                ClaimAmount = @ClaimAmount,
                                Status = @Status,
                                Description = @Description
                            WHERE ClaimId = @ClaimId";
                    }
                    else
                    {
                        // INSERT new claim
                        sql = @"
                            INSERT INTO Claims (ClaimNumber, PolicyHolderName, DateOfIncident, 
                                              DateFiled, ClaimAmount, Status, Description)
                            VALUES (@ClaimNumber, @PolicyHolderName, @DateOfIncident, 
                                   @DateFiled, @ClaimAmount, @Status, @Description)";
                    }

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Add parameters - traditional ADO.NET pattern
                        command.Parameters.AddWithValue("@ClaimNumber", ClaimNumberTextBox.Text.Trim());
                        command.Parameters.AddWithValue("@PolicyHolderName", PolicyHolderNameTextBox.Text.Trim());
                        command.Parameters.AddWithValue("@DateOfIncident", DateOfIncidentPicker.SelectedDate.Value);
                        command.Parameters.AddWithValue("@DateFiled", DateFiledPicker.SelectedDate.Value);
                        command.Parameters.AddWithValue("@ClaimAmount", decimal.Parse(ClaimAmountTextBox.Text));
                        command.Parameters.AddWithValue("@Status", 
                            ((System.Windows.Controls.ComboBoxItem)StatusComboBox.SelectedItem).Content.ToString());
                        command.Parameters.AddWithValue("@Description", 
                            string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? (object)DBNull.Value : DescriptionTextBox.Text.Trim());

                        if (isEditMode)
                        {
                            command.Parameters.AddWithValue("@ClaimId", claimId.Value);
                        }

                        command.ExecuteNonQuery();
                    }
                }

                // Success - close dialog with positive result
                // For edit mode, show confirmation. For add mode, just close and navigate
                if (isEditMode)
                {
                    MessageBox.Show(
                        "Claim updated successfully.",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error saving claim: {ex.Message}",
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Cancel button click handler.
        /// Closes the dialog without saving.
        /// </summary>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Traditional pattern: Ask for confirmation if fields have been modified
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to cancel? Any unsaved changes will be lost.",
                "Confirm Cancel",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.DialogResult = false;
                this.Close();
            }
        }
    }
}
