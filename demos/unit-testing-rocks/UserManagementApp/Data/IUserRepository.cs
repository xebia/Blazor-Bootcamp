namespace UserManagementApp.Data;

/// <summary>
/// Interface defining data access operations for user management.
/// This abstraction allows for easy testing through mocking.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The user's ID</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<User?> GetByIdAsync(int id);
    
    /// <summary>
    /// Retrieves all users from the data store.
    /// </summary>
    /// <returns>A collection of all users</returns>
    Task<IEnumerable<User>> GetAllAsync();
    
    /// <summary>
    /// Adds a new user to the data store.
    /// </summary>
    /// <param name="user">The user to add</param>
    /// <returns>The added user with generated ID</returns>
    Task<User> AddAsync(User user);
    
    /// <summary>
    /// Updates an existing user in the data store.
    /// </summary>
    /// <param name="user">The user with updated information</param>
    /// <returns>True if updated successfully, false otherwise</returns>
    Task<bool> UpdateAsync(User user);
    
    /// <summary>
    /// Deletes a user from the data store.
    /// </summary>
    /// <param name="id">The ID of the user to delete</param>
    /// <returns>True if deleted successfully, false otherwise</returns>
    Task<bool> DeleteAsync(int id);
}
