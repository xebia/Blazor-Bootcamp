using ClaimTrackerBlazor.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ClaimTrackerBlazor.DataAccess;

public class ClaimsDbContext(DbContextOptions<ClaimsDbContext> options) : DbContext(options)
{
    public DbSet<Claim> Claims => Set<Claim>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var claim = modelBuilder.Entity<Claim>();

        claim.ToTable("Claims");
        claim.HasKey(c => c.ClaimId);
        claim.Property(c => c.ClaimNumber).IsRequired().HasMaxLength(50);
        claim.Property(c => c.PolicyHolderName).IsRequired().HasMaxLength(100);
        claim.Property(c => c.DateOfIncident).IsRequired();
        claim.Property(c => c.DateFiled).IsRequired();
        claim.Property(c => c.ClaimAmount).HasColumnType("decimal(18,2)");
        claim.Property(c => c.Status).IsRequired().HasMaxLength(20);
        claim.Property(c => c.Description).HasMaxLength(500);
    }
}
