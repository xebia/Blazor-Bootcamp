using AppServer.Models;
using AppServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace AppServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MuscleCarController : ControllerBase
{
    private readonly MuscleCarService _muscleCarService;

    public MuscleCarController(MuscleCarService muscleCarService)
    {
        _muscleCarService = muscleCarService;
    }

    /// <summary>
    /// Get all muscle cars
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<MuscleCar>>> GetAll()
    {
        var cars = await _muscleCarService.GetAllAsync();
        return Ok(cars);
    }

    /// <summary>
    /// Get a muscle car by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MuscleCar>> GetById(int id)
    {
        var car = await _muscleCarService.GetByIdAsync(id);
        if (car == null)
            return NotFound();

        return Ok(car);
    }

    /// <summary>
    /// Create a new muscle car
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<MuscleCar>> Create([FromBody] MuscleCar muscleCar)
    {
        var createdCar = await _muscleCarService.CreateAsync(muscleCar);
        return CreatedAtAction(nameof(GetById), new { id = createdCar.Id }, createdCar);
    }

    /// <summary>
    /// Update an existing muscle car
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<MuscleCar>> Update(int id, [FromBody] MuscleCar muscleCar)
    {
        var updatedCar = await _muscleCarService.UpdateAsync(id, muscleCar);
        if (updatedCar == null)
            return NotFound();

        return Ok(updatedCar);
    }

    /// <summary>
    /// Delete a muscle car
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _muscleCarService.DeleteAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
