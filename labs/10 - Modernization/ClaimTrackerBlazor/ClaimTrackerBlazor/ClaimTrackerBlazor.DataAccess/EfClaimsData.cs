using ClaimTrackerBlazor.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ClaimTrackerBlazor.DataAccess;

/// <summary>
/// Entity Framework Core implementation of IClaimsData.
/// 
/// TODO: Implement all methods using Entity Framework Core.
/// 
/// This class replaces the ADO.NET data access code from the WPF application.
/// Compare with ClaimTracker\Data\DatabaseManager.cs and the data access code
/// in ClaimsListPage.xaml.cs and AddEditClaimWindow.xaml.cs.
/// 
/// Key differences from the WPF app:
/// - Uses async/await throughout (better for web applications)
/// - Uses DbContext instead of raw SqlConnection/SqlCommand
/// - Uses LINQ instead of SQL strings
/// - Constructor injection for dependencies
/// </summary>
public class EfClaimsData(ClaimsDbContext context) : IClaimsData
{
    private readonly ClaimsDbContext _context = context;

    /// <summary>
    /// Gets all claims ordered by date filed (most recent first).
    /// 
    /// TODO: Implement this method
    /// Hints:
    /// - Use _context.Claims to access the Claims DbSet
    /// - Use AsNoTracking() for read-only queries (better performance)
    /// - Use OrderByDescending() to sort by DateFiled
    /// - Use ToListAsync() to execute the query
    /// </summary>
    public async Task<IEnumerable<Claim>> GetClaimsAsync()
    {
        // TODO: Replace with actual implementation
        throw new NotImplementedException("GetClaimsAsync not yet implemented");
    }

    /// <summary>
    /// Gets a single claim by its ID.
    /// 
    /// TODO: Implement this method
    /// Hints:
    /// - Use FirstOrDefaultAsync() with a predicate
    /// - Return null if not found (the ? in return type allows this)
    /// </summary>
    public Task<Claim?> GetClaimAsync(int id)
    {
        // TODO: Replace with actual implementation
        throw new NotImplementedException("GetClaimAsync not yet implemented");
    }

    /// <summary>
    /// Adds a new claim to the database.
    /// 
    /// TODO: Implement this method
    /// Hints:
    /// - Use _context.Claims.Add() to add the entity
    /// - Use SaveChangesAsync() to persist
    /// - Return the new ClaimId (EF Core populates it after save)
    /// </summary>
    public async Task<int> AddClaimAsync(Claim claim)
    {
        // TODO: Replace with actual implementation
        throw new NotImplementedException("AddClaimAsync not yet implemented");
    }

    /// <summary>
    /// Updates an existing claim.
    /// 
    /// TODO: Implement this method
    /// Hints:
    /// - Use _context.Claims.Update() to mark entity as modified
    /// - Use SaveChangesAsync() to persist
    /// </summary>
    public async Task UpdateClaimAsync(Claim claim)
    {
        // TODO: Replace with actual implementation
        throw new NotImplementedException("UpdateClaimAsync not yet implemented");
    }

    /// <summary>
    /// Deletes a claim by its ID.
    /// 
    /// TODO: Implement this method
    /// Hints:
    /// - First find the entity using FindAsync()
    /// - Check if it exists before removing
    /// - Use _context.Claims.Remove() to delete
    /// - Use SaveChangesAsync() to persist
    /// </summary>
    public async Task DeleteClaimAsync(int id)
    {
        // TODO: Replace with actual implementation
        throw new NotImplementedException("DeleteClaimAsync not yet implemented");
    }
}
