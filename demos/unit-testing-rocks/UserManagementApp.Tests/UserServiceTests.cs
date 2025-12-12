using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocks;
using UserManagementApp.Data;
using UserManagementApp.Services;

[assembly: Rock(typeof(IUserRepository), BuildType.Create)]
[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]

namespace UserManagementApp.Tests;

/// <summary>
/// Unit tests for UserService demonstrating the Rocks mocking framework.
/// Rocks is a compile-time mocking framework that uses source generators.
/// </summary>
[TestClass]
public class UserServiceTests
{
    #region GetUserAsync Tests

    [TestMethod]
    public async Task GetUserAsync_WithValidId_ReturnsUser()
    {
        // Arrange
        var expectedUser = new User
        {
            Id = 1,
            Name = "Test User",
            Email = "test@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Create a mock of IUserRepository using Rocks
        var expectations = new IUserRepositoryCreateExpectations();
        expectations.Setups.GetByIdAsync(1).ReturnValue(Task.FromResult<User?>(expectedUser));
        var repository = expectations.Instance();

        var service = new UserService(repository);

        // Act
        var result = await service.GetUserAsync(1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedUser.Id, result.Id);
        Assert.AreEqual(expectedUser.Name, result.Name);
        Assert.AreEqual(expectedUser.Email, result.Email);

        // Verify that the repository method was called
        expectations.Verify();
    }

    [TestMethod]
    public async Task GetUserAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var expectations = new IUserRepositoryCreateExpectations();
        expectations.Setups.GetByIdAsync(999).ReturnValue(Task.FromResult<User?>(null));
        var repository = expectations.Instance();

        var service = new UserService(repository);

        // Act
        var result = await service.GetUserAsync(999);

        // Assert
        Assert.IsNull(result);
        expectations.Verify();
    }

    [TestMethod]
    public async Task GetUserAsync_WithZeroId_ThrowsArgumentException()
    {
        // Arrange
        var expectations = new IUserRepositoryCreateExpectations();
        var repository = expectations.Instance();
        var service = new UserService(repository);

        // Act & Assert
        try
        {
            await service.GetUserAsync(0);
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException)
        {
            // Expected exception
        }
    }

    [TestMethod]
    public async Task GetUserAsync_WithNegativeId_ThrowsArgumentException()
    {
        // Arrange
        var expectations = new IUserRepositoryCreateExpectations();
        var repository = expectations.Instance();
        var service = new UserService(repository);

        // Act & Assert
        try
        {
            await service.GetUserAsync(-1);
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException)
        {
            // Expected exception
        }
    }

    #endregion

    #region CreateUserAsync Tests

    [TestMethod]
    public async Task CreateUserAsync_WithValidData_CreatesUser()
    {
        // Arrange
        var expectedUser = new User
        {
            Id = 1,
            Name = "New User",
            Email = "new@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var expectations = new IUserRepositoryCreateExpectations();
        expectations.Setups.AddAsync(Arg.Validate<User>(u =>
            u.Name == "New User" &&
            u.Email == "new@example.com" &&
            u.IsActive == true
        )).ReturnValue(Task.FromResult(expectedUser));
        
        var repository = expectations.Instance();
        var service = new UserService(repository);

        // Act
        var result = await service.CreateUserAsync("New User", "new@example.com");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedUser.Id, result.Id);
        Assert.AreEqual(expectedUser.Name, result.Name);
        Assert.AreEqual(expectedUser.Email, result.Email);
        Assert.IsTrue(result.IsActive);
        expectations.Verify();
    }

    [TestMethod]
    public async Task CreateUserAsync_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        var expectations = new IUserRepositoryCreateExpectations();
        var repository = expectations.Instance();
        var service = new UserService(repository);

        // Act & Assert
        try
        {
            await service.CreateUserAsync("", "test@example.com");
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException)
        {
            // Expected exception
        }
    }

    [TestMethod]
    public async Task CreateUserAsync_WithEmptyEmail_ThrowsArgumentException()
    {
        // Arrange
        var expectations = new IUserRepositoryCreateExpectations();
        var repository = expectations.Instance();
        var service = new UserService(repository);

        // Act & Assert
        try
        {
            await service.CreateUserAsync("Test User", "");
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException)
        {
            // Expected exception
        }
    }

    [TestMethod]
    public async Task CreateUserAsync_WithInvalidEmail_ThrowsArgumentException()
    {
        // Arrange
        var expectations = new IUserRepositoryCreateExpectations();
        var repository = expectations.Instance();
        var service = new UserService(repository);

        // Act & Assert
        try
        {
            await service.CreateUserAsync("Test User", "invalid-email");
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException)
        {
            // Expected exception
        }
    }

    #endregion

    #region GetActiveUsersAsync Tests

    [TestMethod]
    public async Task GetActiveUsersAsync_ReturnsOnlyActiveUsers()
    {
        // Arrange
        var allUsers = new List<User>
        {
            new() { Id = 1, Name = "Active User 1", Email = "active1@example.com", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Name = "Inactive User", Email = "inactive@example.com", IsActive = false, CreatedAt = DateTime.UtcNow },
            new() { Id = 3, Name = "Active User 2", Email = "active2@example.com", IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        var expectations = new IUserRepositoryCreateExpectations();
        expectations.Setups.GetAllAsync().ReturnValue(Task.FromResult<IEnumerable<User>>(allUsers));
        var repository = expectations.Instance();

        var service = new UserService(repository);

        // Act
        var result = await service.GetActiveUsersAsync();
        var activeUsers = result.ToList();

        // Assert
        Assert.AreEqual(2, activeUsers.Count); // MSTest 4.0 recommended: use Assert.AreEqual for count
        Assert.IsTrue(activeUsers.All(u => u.IsActive));
        Assert.IsTrue(activeUsers.Any(u => u.Id == 1));
        Assert.IsTrue(activeUsers.Any(u => u.Id == 3));
        Assert.IsFalse(activeUsers.Any(u => u.Id == 2));
        expectations.Verify();
    }

    [TestMethod]
    public async Task GetActiveUsersAsync_WithNoActiveUsers_ReturnsEmptyList()
    {
        // Arrange
        var allUsers = new List<User>
        {
            new() { Id = 1, Name = "Inactive User 1", Email = "inactive1@example.com", IsActive = false, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Name = "Inactive User 2", Email = "inactive2@example.com", IsActive = false, CreatedAt = DateTime.UtcNow }
        };

        var expectations = new IUserRepositoryCreateExpectations();
        expectations.Setups.GetAllAsync().ReturnValue(Task.FromResult<IEnumerable<User>>(allUsers));
        var repository = expectations.Instance();

        var service = new UserService(repository);

        // Act
        var result = await service.GetActiveUsersAsync();
        var activeUsers = result.ToList();

        // Assert
        Assert.AreEqual(0, activeUsers.Count); // MSTest 4.0 recommended: use Assert.AreEqual for count
        expectations.Verify();
    }

    #endregion

    #region DeactivateUserAsync Tests

    [TestMethod]
    public async Task DeactivateUserAsync_WithExistingUser_DeactivatesAndReturnsTrue()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Name = "Test User",
            Email = "test@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var expectations = new IUserRepositoryCreateExpectations();
        expectations.Setups.GetByIdAsync(1).ReturnValue(Task.FromResult<User?>(user));
        expectations.Setups.UpdateAsync(Arg.Validate<User>(u =>
            u.Id == 1 &&
            u.IsActive == false
        )).ReturnValue(Task.FromResult(true));
        
        var repository = expectations.Instance();
        var service = new UserService(repository);

        // Act
        var result = await service.DeactivateUserAsync(1);

        // Assert
        Assert.IsTrue(result);
        Assert.IsFalse(user.IsActive);
        expectations.Verify();
    }

    [TestMethod]
    public async Task DeactivateUserAsync_WithNonExistentUser_ReturnsFalse()
    {
        // Arrange
        var expectations = new IUserRepositoryCreateExpectations();
        expectations.Setups.GetByIdAsync(999).ReturnValue(Task.FromResult<User?>(null));
        var repository = expectations.Instance();

        var service = new UserService(repository);

        // Act
        var result = await service.DeactivateUserAsync(999);

        // Assert
        Assert.IsFalse(result);
        expectations.Verify();
    }

    [TestMethod]
    public async Task DeactivateUserAsync_WithInvalidId_ThrowsArgumentException()
    {
        // Arrange
        var expectations = new IUserRepositoryCreateExpectations();
        var repository = expectations.Instance();
        var service = new UserService(repository);

        // Act & Assert
        try
        {
            await service.DeactivateUserAsync(0);
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException)
        {
            // Expected exception
        }
    }

    #endregion

    #region GetStatisticsAsync Tests

    [TestMethod]
    public async Task GetStatisticsAsync_WithMixedUsers_ReturnsCorrectStatistics()
    {
        // Arrange
        var allUsers = new List<User>
        {
            new() { Id = 1, Name = "Active 1", Email = "active1@example.com", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Name = "Active 2", Email = "active2@example.com", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 3, Name = "Active 3", Email = "active3@example.com", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 4, Name = "Inactive 1", Email = "inactive1@example.com", IsActive = false, CreatedAt = DateTime.UtcNow },
            new() { Id = 5, Name = "Inactive 2", Email = "inactive2@example.com", IsActive = false, CreatedAt = DateTime.UtcNow }
        };

        var expectations = new IUserRepositoryCreateExpectations();
        expectations.Setups.GetAllAsync().ReturnValue(Task.FromResult<IEnumerable<User>>(allUsers));
        var repository = expectations.Instance();

        var service = new UserService(repository);

        // Act
        var stats = await service.GetStatisticsAsync();

        // Assert
        Assert.AreEqual(5, stats.TotalUsers);
        Assert.AreEqual(3, stats.ActiveUsers);
        Assert.AreEqual(2, stats.InactiveUsers);
        expectations.Verify();
    }

    [TestMethod]
    public async Task GetStatisticsAsync_WithNoUsers_ReturnsZeroStatistics()
    {
        // Arrange
        var expectations = new IUserRepositoryCreateExpectations();
        expectations.Setups.GetAllAsync().ReturnValue(Task.FromResult<IEnumerable<User>>(new List<User>()));
        var repository = expectations.Instance();

        var service = new UserService(repository);

        // Act
        var stats = await service.GetStatisticsAsync();

        // Assert
        Assert.AreEqual(0, stats.TotalUsers);
        Assert.AreEqual(0, stats.ActiveUsers);
        Assert.AreEqual(0, stats.InactiveUsers);
        expectations.Verify();
    }

    #endregion

    #region Constructor Tests

    [TestMethod]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        try
        {
            var service = new UserService(null!);
            Assert.Fail("Expected ArgumentNullException was not thrown");
        }
        catch (ArgumentNullException)
        {
            // Expected exception
        }
    }

    #endregion
}
