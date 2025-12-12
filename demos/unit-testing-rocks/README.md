# Unit Testing with MSTest and Rocks Mocking Framework

This demo showcases modern unit testing practices in .NET 10 using **MSTest** as the testing framework and **Rocks** for mocking. The solution demonstrates test-driven development principles, dependency injection, and interface-based design.

## Overview

The demo consists of a simple user management system with:
- **Console Application**: Demonstrates the business logic and data access layer in action
- **Test Project**: Comprehensive unit tests showing how to mock the data access layer using Rocks

## What is Rocks?

[Rocks](https://github.com/JasonBock/Rocks) is a modern .NET mocking library that uses **source generators** and the Roslyn Compiler APIs to create mocks at compile time. Unlike runtime-based mocking libraries (like Moq or NSubstitute), Rocks generates C# code that you can step into during debugging.

### Key Benefits of Rocks:
- ✅ **Compile-time mock generation** - No reflection or IL emit
- ✅ **Debuggable** - Step into generated mock code
- ✅ **Type-safe** - Leverages the compiler for validation
- ✅ **Modern C# features** - Supports records, init properties, etc.
- ✅ **Performance** - No runtime overhead

## Solution Structure

```
unit-testing-rocks/
├── UserManagementApp/          # Main console application
│   ├── Data/
│   │   ├── IUserRepository.cs      # Repository interface
│   │   └── InMemoryUserRepository.cs   # Repository implementation
│   ├── Models/
│   │   └── User.cs                 # User model
│   ├── Services/
│   │   └── UserService.cs          # Business logic layer
│   └── Program.cs                  # Application entry point with DI
│
└── UserManagementApp.Tests/    # Unit test project
    └── UserServiceTests.cs         # Comprehensive unit tests
```

## Technologies Used

- **.NET 10** - Latest .NET framework
- **MSTest 4.0.1** - Microsoft's unit testing framework
- **Rocks 10.0.0** - Source generator-based mocking framework
- **Microsoft.Extensions.Hosting** - Dependency injection and hosting
- **C# 13** - Latest C# language features

## Key Concepts Demonstrated

### 1. Interface-Based Design
The application uses `IUserRepository` interface to decouple the business logic from the data access implementation:

```csharp
public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> AddAsync(User user);
    Task<bool> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
}
```

### 2. Dependency Injection
The application uses Microsoft's DI container:

```csharp
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddScoped<UserService>();
    })
    .Build();
```

### 3. Business Logic Layer
`UserService` contains the business rules and depends on the repository abstraction:

```csharp
public class UserService
{
    private readonly IUserRepository _repository;
    
    public UserService(IUserRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    
    // Business methods with validation logic
}
```

### 4. Mocking with Rocks

Rocks uses an assembly-level attribute to generate mock infrastructure:

```csharp
[assembly: Rock(typeof(IUserRepository), BuildType.Create)]
```

This generates a class named `IUserRepositoryCreateExpectations` that you use in tests:

```csharp
// Arrange
var expectations = new IUserRepositoryCreateExpectations();
expectations.Setups.GetByIdAsync(1).ReturnValue(Task.FromResult<User?>(expectedUser));
var repository = expectations.Instance();

// Act
var service = new UserService(repository);
var result = await service.GetUserAsync(1);

// Assert
Assert.IsNotNull(result);
expectations.Verify(); // Verifies all expectations were met
```

## Test Coverage

The test suite includes 16 comprehensive tests covering:

### ✅ Happy Path Tests
- Getting users by ID
- Creating users with valid data
- Filtering active users
- Deactivating users
- Calculating statistics

### ✅ Validation Tests
- Invalid ID values (zero, negative)
- Empty/null parameters
- Invalid email formats

### ✅ Edge Cases
- Non-existent users
- Empty user lists
- Update failures

### ✅ Constructor Tests
- Null dependency handling

## Running the Project

### Run the Console Application
```bash
cd unit-testing-rocks
dotnet run --project UserManagementApp
```

### Run the Tests
```bash
cd unit-testing-rocks
dotnet test
```

### Build the Solution
```bash
cd unit-testing-rocks
dotnet build
```

## Sample Test: Mocking with Parameter Validation

Here's an example showing Rocks' powerful parameter validation:

```csharp
[TestMethod]
public async Task CreateUserAsync_WithValidData_CreatesUser()
{
    // Arrange
    var expectedUser = new User
    {
        Id = 1,
        Name = "New User",
        Email = "new@example.com",
        IsActive = true
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
    expectations.Verify();
}
```

## Rocks Features Demonstrated

### 1. **Method Expectations**
```csharp
expectations.Setups.GetByIdAsync(1).ReturnValue(Task.FromResult<User?>(user));
```

### 2. **Parameter Validation**
```csharp
expectations.Setups.AddAsync(Arg.Validate<User>(u => u.IsActive)).ReturnValue(result);
```

### 3. **Any Parameter Matching**
```csharp
expectations.Setups.GetByIdAsync(Arg.Any<int>()).ReturnValue(Task.FromResult<User?>(user));
```

### 4. **Verification**
```csharp
expectations.Verify(); // Ensures all setups were called as expected
```

## Best Practices Highlighted

1. **Arrange-Act-Assert Pattern** - Clear test structure
2. **Single Responsibility** - Each test validates one behavior
3. **Descriptive Test Names** - Method names explain what's being tested
4. **Interface Segregation** - Repository interface is focused and testable
5. **Dependency Injection** - Loose coupling enables testing
6. **Async/Await** - Modern asynchronous programming patterns
7. **Strict Mocking** - All expectations must be met (Rocks default)

## Comparison with Other Mocking Frameworks

| Feature | Rocks | Moq | NSubstitute |
|---------|-------|-----|-------------|
| Mock Generation | Compile-time (source gen) | Runtime (IL emit) | Runtime (Castle proxy) |
| Debuggability | ✅ Full | ❌ Limited | ❌ Limited |
| Performance | ✅ Fast | ⚠️ Moderate | ⚠️ Moderate |
| .NET 10 Support | ✅ Yes | ⚠️ Partial | ⚠️ Partial |
| Learning Curve | ⚠️ Moderate | ✅ Easy | ✅ Easy |
| Type Safety | ✅ Strong | ✅ Strong | ✅ Strong |

## When to Use Rocks

✅ **Good fit for:**
- New projects with modern .NET
- Teams wanting debuggable mocks
- Performance-critical scenarios
- Learning modern source generators

⚠️ **Consider alternatives if:**
- Working with legacy .NET Framework
- Team is already invested in another framework
- Need extremely flexible/dynamic mocking scenarios

## Additional Resources

- [Rocks GitHub Repository](https://github.com/JasonBock/Rocks)
- [Rocks Documentation](https://github.com/JasonBock/Rocks/blob/main/docs/Overview.md)
- [MSTest Documentation](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-mstest)
- [Unit Testing Best Practices](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)

## License

This demo is part of the Blazor Bootcamp training materials.

---

**Note**: This is a demonstration project focused on teaching unit testing concepts. In a production application, you would typically use a real database with Entity Framework Core or another ORM.
