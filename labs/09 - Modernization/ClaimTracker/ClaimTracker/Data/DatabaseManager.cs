using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace ClaimTracker.Data
{
    /// <summary>
    /// Manages database creation and initialization.
    /// This demonstrates a traditional ADO.NET approach common in legacy WPF applications.
    /// </summary>
    public static class DatabaseManager
    {
        private static readonly string ConnectionString = 
            ConfigurationManager.ConnectionStrings["ClaimsDB"].ConnectionString;

        /// <summary>
        /// Ensures the database exists and is properly initialized.
        /// Creates the database and seeds sample data on first run.
        /// </summary>
        public static void InitializeDatabase()
        {
            try
            {
                // Set up the data directory
                string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
                AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);

                // Ensure App_Data directory exists
                if (!Directory.Exists(dataDirectory))
                {
                    Directory.CreateDirectory(dataDirectory);
                }

                string dbFilePath = Path.Combine(dataDirectory, "ClaimsTracker.mdf");
                bool isNewDatabase = !File.Exists(dbFilePath);

                if (isNewDatabase)
                {
                    // Create the database file first using master connection
                    CreateDatabase(dataDirectory, dbFilePath);
                }

                // Now connect to the database and create schema/seed data if needed
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    // Check if Claims table exists
                    bool tableExists = CheckIfTableExists(connection);

                    if (!tableExists)
                    {
                        CreateSchema(connection);
                        SeedData(connection);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to initialize database: {ex.Message}\n\nInner Exception: {ex.InnerException?.Message}", ex);
            }
        }

        /// <summary>
        /// Creates the database file using a master database connection.
        /// </summary>
        private static void CreateDatabase(string dataDirectory, string dbFilePath)
        {
            // Connect to master to create the database
            string masterConnectionString = 
                @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30";

            string createDbSql = $@"
                CREATE DATABASE [ClaimsTracker]
                ON PRIMARY (
                    NAME = ClaimsTracker,
                    FILENAME = '{dbFilePath}'
                )";

            using (SqlConnection connection = new SqlConnection(masterConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(createDbSql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            // Detach the database so it can be attached via the connection string
            using (SqlConnection connection = new SqlConnection(masterConnectionString))
            {
                connection.Open();
                string detachSql = "EXEC sp_detach_db 'ClaimsTracker', 'true'";
                using (SqlCommand command = new SqlCommand(detachSql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Checks if the Claims table exists in the database.
        /// </summary>
        private static bool CheckIfTableExists(SqlConnection connection)
        {
            string checkTableSql = @"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_NAME = 'Claims'";

            using (SqlCommand command = new SqlCommand(checkTableSql, connection))
            {
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        /// <summary>
        /// Creates the Claims table schema.
        /// </summary>
        private static void CreateSchema(SqlConnection connection)
        {
            string createTableSql = @"
                CREATE TABLE Claims (
                    ClaimId INT PRIMARY KEY IDENTITY(1,1),
                    ClaimNumber NVARCHAR(50) NOT NULL,
                    PolicyHolderName NVARCHAR(100) NOT NULL,
                    DateOfIncident DATETIME NOT NULL,
                    DateFiled DATETIME NOT NULL,
                    ClaimAmount DECIMAL(18,2) NOT NULL,
                    Status NVARCHAR(20) NOT NULL,
                    Description NVARCHAR(500) NULL
                );";

            using (SqlCommand command = new SqlCommand(createTableSql, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Seeds the database with sample claims data for demonstration purposes.
        /// </summary>
        private static void SeedData(SqlConnection connection)
        {
            string insertSql = @"
                INSERT INTO Claims (ClaimNumber, PolicyHolderName, DateOfIncident, DateFiled, ClaimAmount, Status, Description)
                VALUES (@ClaimNumber, @PolicyHolderName, @DateOfIncident, @DateFiled, @ClaimAmount, @Status, @Description);";

            // Sample claims data
            var sampleClaims = new[]
            {
                new
                {
                    ClaimNumber = "CLM-2024-001",
                    PolicyHolderName = "John Smith",
                    DateOfIncident = DateTime.Today.AddDays(-30),
                    DateFiled = DateTime.Today.AddDays(-25),
                    ClaimAmount = 1500.00m,
                    Status = "Pending",
                    Description = "Minor vehicle damage from parking lot incident"
                },
                new
                {
                    ClaimNumber = "CLM-2024-002",
                    PolicyHolderName = "Sarah Johnson",
                    DateOfIncident = DateTime.Today.AddDays(-45),
                    DateFiled = DateTime.Today.AddDays(-40),
                    ClaimAmount = 3200.50m,
                    Status = "Approved",
                    Description = "Home water damage from burst pipe"
                },
                new
                {
                    ClaimNumber = "CLM-2024-003",
                    PolicyHolderName = "Michael Chen",
                    DateOfIncident = DateTime.Today.AddDays(-60),
                    DateFiled = DateTime.Today.AddDays(-58),
                    ClaimAmount = 850.75m,
                    Status = "Processing",
                    Description = "Bicycle theft from secured garage"
                },
                new
                {
                    ClaimNumber = "CLM-2024-004",
                    PolicyHolderName = "Emily Davis",
                    DateOfIncident = DateTime.Today.AddDays(-20),
                    DateFiled = DateTime.Today.AddDays(-18),
                    ClaimAmount = 4500.00m,
                    Status = "Pending",
                    Description = "Storm damage to roof and siding"
                },
                new
                {
                    ClaimNumber = "CLM-2024-005",
                    PolicyHolderName = "Robert Martinez",
                    DateOfIncident = DateTime.Today.AddDays(-90),
                    DateFiled = DateTime.Today.AddDays(-85),
                    ClaimAmount = 2100.25m,
                    Status = "Denied",
                    Description = "Vehicle accident - policy lapsed at time of incident"
                },
                new
                {
                    ClaimNumber = "CLM-2024-006",
                    PolicyHolderName = "Jennifer Wilson",
                    DateOfIncident = DateTime.Today.AddDays(-15),
                    DateFiled = DateTime.Today.AddDays(-10),
                    ClaimAmount = 6750.00m,
                    Status = "Processing",
                    Description = "Fire damage to kitchen from cooking accident"
                },
                new
                {
                    ClaimNumber = "CLM-2024-007",
                    PolicyHolderName = "David Brown",
                    DateOfIncident = DateTime.Today.AddDays(-5),
                    DateFiled = DateTime.Today.AddDays(-3),
                    ClaimAmount = 1200.00m,
                    Status = "Pending",
                    Description = "Laptop damaged due to accidental drop"
                },
                new
                {
                    ClaimNumber = "CLM-2024-008",
                    PolicyHolderName = "Lisa Anderson",
                    DateOfIncident = DateTime.Today.AddDays(-70),
                    DateFiled = DateTime.Today.AddDays(-65),
                    ClaimAmount = 5500.00m,
                    Status = "Approved",
                    Description = "Medical expenses from slip and fall incident"
                }
            };

            foreach (var claim in sampleClaims)
            {
                using (SqlCommand command = new SqlCommand(insertSql, connection))
                {
                    command.Parameters.AddWithValue("@ClaimNumber", claim.ClaimNumber);
                    command.Parameters.AddWithValue("@PolicyHolderName", claim.PolicyHolderName);
                    command.Parameters.AddWithValue("@DateOfIncident", claim.DateOfIncident);
                    command.Parameters.AddWithValue("@DateFiled", claim.DateFiled);
                    command.Parameters.AddWithValue("@ClaimAmount", claim.ClaimAmount);
                    command.Parameters.AddWithValue("@Status", claim.Status);
                    command.Parameters.AddWithValue("@Description", claim.Description);
                    
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
