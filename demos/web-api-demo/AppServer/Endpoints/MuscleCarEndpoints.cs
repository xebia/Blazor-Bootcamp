using AppServer.Models;
using AppServer.Services;

namespace AppServer.Endpoints;

public static class MuscleCarEndpoints
{
    public static void MapMuscleCarEndpoints(this IEndpointRouteBuilder app)
    {
        var muscleCarGroup = app.MapGroup("/minimal/musclecars")
            .WithTags("MuscleCar Minimal API");

        muscleCarGroup.MapGet("/", async (MuscleCarService service) =>
        {
            var cars = await service.GetAllAsync();
            return Results.Ok(cars);
        })
        .WithDescription("Get all muscle cars")
        .WithSummary("Returns a list of all muscle cars with their horsepower calculations");

        // Example of using ProblemDetails for standardized error responses (RFC 7807).
        // Instead of just returning NotFound(), we return a structured error with
        // detail, title, and status that clients can parse consistently.
        muscleCarGroup.MapGet("/{id}", async (int id, MuscleCarService service) =>
        {
            var car = await service.GetByIdAsync(id);
            
            if (car is not null)
                return Results.Ok(car);
            
            return Results.Problem(
                detail: $"Muscle car with ID {id} was not found.",
                statusCode: StatusCodes.Status404NotFound,
                title: "Muscle Car Not Found");
        })
        .WithName("GetMuscleCarById")
        .WithDescription("Get a muscle car by ID")
        .WithSummary("Returns a single muscle car by its unique identifier");

        muscleCarGroup.MapPost("/", async (MuscleCar muscleCar, MuscleCarService service) =>
        {
            var createdCar = await service.CreateAsync(muscleCar);

            // When a POST creates a new resource, HTTP conventions say the
            // response should include a Location header pointing to where
            // that resource can be retrieved. CreatedAtRoute just ensures
            // that URL is generated correctly from the actual GET endpoint
            // instead of being hardcoded (uses .WithName("GetMuscleCarById") above)
            return Results.CreatedAtRoute(
                "GetMuscleCarById",
                new { id = createdCar.Id },
                createdCar);
        })
        .WithDescription("Create a new muscle car")
        .WithSummary("Creates a new muscle car entry and calculates its horsepower");


        muscleCarGroup.MapPut("/{id}", async (int id, MuscleCar muscleCar, MuscleCarService service) =>
        {
            var updatedCar = await service.UpdateAsync(id, muscleCar);
            return updatedCar != null ? Results.Ok(updatedCar) : Results.NotFound();
        })
        .WithDescription("Update an existing muscle car")
        .WithSummary("Updates the details of an existing muscle car");

        muscleCarGroup.MapDelete("/{id}", async (int id, MuscleCarService service) =>
        {
            var result = await service.DeleteAsync(id);
            return result ? Results.NoContent() : Results.NotFound();
        })
        .WithDescription("Delete a muscle car")
        .WithSummary("Removes a muscle car from the collection");
    }
}
