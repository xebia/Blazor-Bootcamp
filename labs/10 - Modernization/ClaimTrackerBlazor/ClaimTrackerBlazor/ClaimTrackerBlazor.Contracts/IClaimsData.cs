namespace ClaimTrackerBlazor.Contracts;

public interface IClaimsData
{
    Task<IEnumerable<Claim>> GetClaimsAsync();
    Task<Claim?> GetClaimAsync(int id);
    Task<int> AddClaimAsync(Claim claim);
    Task UpdateClaimAsync(Claim claim);
    Task DeleteClaimAsync(int id);
}
