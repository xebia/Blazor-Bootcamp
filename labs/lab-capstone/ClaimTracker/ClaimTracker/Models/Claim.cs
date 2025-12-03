using System;

namespace ClaimTracker.Models
{
    /// <summary>
    /// Represents an insurance claim in the system.
    /// This is a simple POCO (Plain Old CLR Object) - typical of legacy WPF apps.
    /// </summary>
    public class Claim
    {
        public int ClaimId { get; set; }
        public string ClaimNumber { get; set; }
        public string PolicyHolderName { get; set; }
        public DateTime DateOfIncident { get; set; }
        public DateTime DateFiled { get; set; }
        public decimal ClaimAmount { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }
}
