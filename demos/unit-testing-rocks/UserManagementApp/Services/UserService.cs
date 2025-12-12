using UserManagementApp.Data;

namespace UserManagementApp.Services;

/// <summary>
/// Service that handles business logic for user management operations.
/// This layer sits between the application and the data access layer.
/// </summary>
public class UserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <summary>
    /// Gets a user by ID with validation.
    /// </summary>
    public async Task<User?> GetUserAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("User ID must be greater than zero.", nameof(id));
        }

        return await _repository.GetByIdAsync(id);
    }

    /// <summary>
    /// Gets all active users.
    /// </summary>
    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        var allUsers = await _repository.GetAllAsync();
        return allUsers.Where(u => u.IsActive);
    }

    /// <summary>
    /// Creates a new user with validation.
    /// </summary>
    public async Task<User> CreateUserAsync(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.", nameof(email));
        }

        if (!email.Contains('@'))
        {
            throw new ArgumentException("Email must be valid.", nameof(email));
        }

        var user = new User
        {
            Name = name,
            Email = email,
            IsActive = true
        };

        return await _repository.AddAsync(user);
    }

    /// <summary>
    /// Deactivates a user instead of deleting them.
    /// </summary>
    public async Task<bool> DeactivateUserAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("User ID must be greater than zero.", nameof(id));
        }

        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        user.IsActive = false;
        return await _repository.UpdateAsync(user);
    }

    /// <summary>
    /// Gets user statistics.
    /// </summary>
    public async Task<UserStatistics> GetStatisticsAsync()
    {
        var allUsers = await _repository.GetAllAsync();
        var userList = allUsers.ToList();

        return new UserStatistics
        {
            TotalUsers = userList.Count,
            ActiveUsers = userList.Count(u => u.IsActive),
            InactiveUsers = userList.Count(u => !u.IsActive)
        };
    }
}

/// <summary>
/// Represents user statistics.
/// </summary>
public class UserStatistics
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
}
