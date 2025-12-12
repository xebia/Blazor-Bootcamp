using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserManagementApp.Data;
using UserManagementApp.Services;

// Create the host with dependency injection
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Register the data access layer
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        
        // Register the business logic layer
        services.AddScoped<UserService>();
    })
    .Build();

// Get the UserService from DI container
var userService = host.Services.GetRequiredService<UserService>();

Console.WriteLine("=== User Management System Demo ===");
Console.WriteLine();

try
{
    // Create some users
    Console.WriteLine("Creating users...");
    var user1 = await userService.CreateUserAsync("Alice Johnson", "alice@example.com");
    Console.WriteLine($"Created: {user1.Name} ({user1.Email}) - ID: {user1.Id}");
    
    var user2 = await userService.CreateUserAsync("Bob Smith", "bob@example.com");
    Console.WriteLine($"Created: {user2.Name} ({user2.Email}) - ID: {user2.Id}");
    
    var user3 = await userService.CreateUserAsync("Charlie Brown", "charlie@example.com");
    Console.WriteLine($"Created: {user3.Name} ({user3.Email}) - ID: {user3.Id}");
    Console.WriteLine();

    // Get all active users
    Console.WriteLine("Active users:");
    var activeUsers = await userService.GetActiveUsersAsync();
    foreach (var user in activeUsers)
    {
        Console.WriteLine($"  - {user.Name} ({user.Email})");
    }
    Console.WriteLine();

    // Deactivate a user
    Console.WriteLine($"Deactivating user {user2.Id}...");
    var deactivated = await userService.DeactivateUserAsync(user2.Id);
    Console.WriteLine($"Deactivation {(deactivated ? "successful" : "failed")}");
    Console.WriteLine();

    // Get statistics
    Console.WriteLine("User statistics:");
    var stats = await userService.GetStatisticsAsync();
    Console.WriteLine($"  Total Users: {stats.TotalUsers}");
    Console.WriteLine($"  Active Users: {stats.ActiveUsers}");
    Console.WriteLine($"  Inactive Users: {stats.InactiveUsers}");
    Console.WriteLine();

    // Get a specific user
    Console.WriteLine($"Retrieving user {user1.Id}...");
    var retrievedUser = await userService.GetUserAsync(user1.Id);
    if (retrievedUser != null)
    {
        Console.WriteLine($"Found: {retrievedUser.Name} - Active: {retrievedUser.IsActive}");
    }
    Console.WriteLine();

    Console.WriteLine("Demo completed successfully!");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Validation error: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}
