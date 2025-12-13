using ClaimTrackerBlazor.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ClaimTrackerBlazor.DataAccess;

public static class ClaimsDbInitializer
{
    public static async Task InitializeAsync(ClaimsDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Claims.AnyAsync())
        {
            return;
        }

        var today = DateTime.UtcNow.Date;
        var seedClaims = new List<Claim>
        {
            new()
            {
                ClaimNumber = "CLM-2024-001",
                PolicyHolderName = "John Smith",
                DateOfIncident = today.AddDays(-30),
                DateFiled = today.AddDays(-25),
                ClaimAmount = 1500.00m,
                Status = "Pending",
                Description = "Minor vehicle damage from parking lot incident"
            },
            new()
            {
                ClaimNumber = "CLM-2024-002",
                PolicyHolderName = "Sarah Johnson",
                DateOfIncident = today.AddDays(-45),
                DateFiled = today.AddDays(-40),
                ClaimAmount = 3200.50m,
                Status = "Approved",
                Description = "Home water damage from burst pipe"
            },
            new()
            {
                ClaimNumber = "CLM-2024-003",
                PolicyHolderName = "Michael Chen",
                DateOfIncident = today.AddDays(-60),
                DateFiled = today.AddDays(-58),
                ClaimAmount = 850.75m,
                Status = "Processing",
                Description = "Bicycle theft from secured garage"
            },
            new()
            {
                ClaimNumber = "CLM-2024-004",
                PolicyHolderName = "Emily Davis",
                DateOfIncident = today.AddDays(-20),
                DateFiled = today.AddDays(-18),
                ClaimAmount = 4500.00m,
                Status = "Pending",
                Description = "Storm damage to roof and siding"
            },
            new()
            {
                ClaimNumber = "CLM-2024-005",
                PolicyHolderName = "Robert Martinez",
                DateOfIncident = today.AddDays(-90),
                DateFiled = today.AddDays(-85),
                ClaimAmount = 2100.25m,
                Status = "Denied",
                Description = "Vehicle accident - policy lapsed at time of incident"
            },
            new()
            {
                ClaimNumber = "CLM-2024-006",
                PolicyHolderName = "Jennifer Wilson",
                DateOfIncident = today.AddDays(-15),
                DateFiled = today.AddDays(-10),
                ClaimAmount = 6750.00m,
                Status = "Processing",
                Description = "Fire damage to kitchen from cooking accident"
            },
            new()
            {
                ClaimNumber = "CLM-2024-007",
                PolicyHolderName = "David Brown",
                DateOfIncident = today.AddDays(-5),
                DateFiled = today.AddDays(-3),
                ClaimAmount = 1200.00m,
                Status = "Pending",
                Description = "Laptop damaged due to accidental drop"
            },
            new()
            {
                ClaimNumber = "CLM-2024-008",
                PolicyHolderName = "Lisa Anderson",
                DateOfIncident = today.AddDays(-70),
                DateFiled = today.AddDays(-65),
                ClaimAmount = 5500.00m,
                Status = "Approved",
                Description = "Medical expenses from slip and fall incident"
            }
        };

        context.Claims.AddRange(seedClaims);
        await context.SaveChangesAsync();
    }
}
