using ClaimTrackerBlazor.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ClaimTrackerBlazor.DataAccess;

public class EfClaimsData(ClaimsDbContext context) : IClaimsData
{
    private readonly ClaimsDbContext _context = context;

    public async Task<IEnumerable<Claim>> GetClaimsAsync()
    {
        return await _context.Claims
            .AsNoTracking()
            .OrderByDescending(c => c.DateFiled)
            .ToListAsync();
    }

    public Task<Claim?> GetClaimAsync(int id)
    {
        return _context.Claims
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ClaimId == id);
    }

    public async Task<int> AddClaimAsync(Claim claim)
    {
        _context.Claims.Add(claim);
        await _context.SaveChangesAsync();
        return claim.ClaimId;
    }

    public async Task UpdateClaimAsync(Claim claim)
    {
        _context.Claims.Update(claim);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteClaimAsync(int id)
    {
        var entity = await _context.Claims.FindAsync(id);
        if (entity == null)
        {
            return;
        }

        _context.Claims.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
