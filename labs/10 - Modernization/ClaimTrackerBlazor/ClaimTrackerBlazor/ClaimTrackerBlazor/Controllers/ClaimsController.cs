using ClaimTrackerBlazor.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ClaimTrackerBlazor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClaimsController(IClaimsData claimsData) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<Claim>> Get()
    {
        return await claimsData.GetClaimsAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Claim>> GetClaim(int id)
    {
        var claim = await claimsData.GetClaimAsync(id);
        if (claim == null)
        {
            return NotFound();
        }

        return claim;
    }

    [HttpPost]
    public async Task<ActionResult<Claim>> Post(Claim claim)
    {
        if (claim.ClaimId == 0)
        {
            var newId = await claimsData.AddClaimAsync(claim);
            claim.ClaimId = newId;
            return CreatedAtAction(nameof(GetClaim), new { id = newId }, claim);
        }

        await claimsData.UpdateClaimAsync(claim);
        return Ok(claim);
    }

    [HttpPut]
    public async Task<ActionResult<int>> Put(Claim claim)
    {
        var newId = await claimsData.AddClaimAsync(claim);
        return Ok(newId);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await claimsData.DeleteClaimAsync(id);
        return NoContent();
    }
}
