using AppServer.Models;

namespace AppServer.Services;

public class MuscleCarService
{
    private readonly List<MuscleCar> _muscleCars = new()
    {
        new MuscleCar 
        { 
            Id = 1, 
            Make = "Chevrolet", 
            Model = "Chevelle SS 454", 
            Year = 1970, 
            Weight = 4000, 
            QuarterMileTime = 13.12, 
            QuarterMileSpeed = 107.1 
        },
        new MuscleCar 
        { 
            Id = 2, 
            Make = "Plymouth", 
            Model = "Hemi Cuda", 
            Year = 1970, 
            Weight = 3895, 
            QuarterMileTime = 13.10, 
            QuarterMileSpeed = 107.0 
        },
        new MuscleCar 
        { 
            Id = 3, 
            Make = "Dodge", 
            Model = "Charger R/T", 
            Year = 1969, 
            Weight = 3700, 
            QuarterMileTime = 13.50, 
            QuarterMileSpeed = 105.5 
        },
        new MuscleCar 
        { 
            Id = 4, 
            Make = "Pontiac", 
            Model = "GTO Judge", 
            Year = 1969, 
            Weight = 3830, 
            QuarterMileTime = 13.90, 
            QuarterMileSpeed = 103.3 
        },
        new MuscleCar 
        { 
            Id = 5, 
            Make = "Ford", 
            Model = "Mustang Boss 429", 
            Year = 1969, 
            Weight = 3870, 
            QuarterMileTime = 13.60, 
            QuarterMileSpeed = 106.0 
        },
        new MuscleCar 
        { 
            Id = 6, 
            Make = "Chevrolet", 
            Model = "Camaro Z28", 
            Year = 1969, 
            Weight = 3640, 
            QuarterMileTime = 14.00, 
            QuarterMileSpeed = 101.4 
        },
        new MuscleCar 
        { 
            Id = 7, 
            Make = "Oldsmobile", 
            Model = "442 W-30", 
            Year = 1970, 
            Weight = 3950, 
            QuarterMileTime = 13.80, 
            QuarterMileSpeed = 104.2 
        },
        new MuscleCar 
        { 
            Id = 8, 
            Make = "Buick", 
            Model = "GSX Stage 1", 
            Year = 1970, 
            Weight = 4100, 
            QuarterMileTime = 13.38, 
            QuarterMileSpeed = 105.5 
        },
        new MuscleCar 
        { 
            Id = 9, 
            Make = "Mercury", 
            Model = "Cyclone Spoiler II", 
            Year = 1969, 
            Weight = 3800, 
            QuarterMileTime = 14.20, 
            QuarterMileSpeed = 100.3 
        },
        new MuscleCar 
        { 
            Id = 10, 
            Make = "AMC", 
            Model = "Javelin AMX", 
            Year = 1970, 
            Weight = 3650, 
            QuarterMileTime = 14.40, 
            QuarterMileSpeed = 99.5 
        }
    };

    public Task<List<MuscleCar>> GetAllAsync()
    {
        return Task.FromResult(_muscleCars);
    }

    public Task<MuscleCar?> GetByIdAsync(int id)
    {
        var car = _muscleCars.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(car);
    }

    public Task<MuscleCar> CreateAsync(MuscleCar muscleCar)
    {
        muscleCar.Id = _muscleCars.Max(c => c.Id) + 1;
        _muscleCars.Add(muscleCar);
        return Task.FromResult(muscleCar);
    }

    public Task<MuscleCar?> UpdateAsync(int id, MuscleCar muscleCar)
    {
        var existingCar = _muscleCars.FirstOrDefault(c => c.Id == id);
        if (existingCar == null)
            return Task.FromResult<MuscleCar?>(null);

        existingCar.Make = muscleCar.Make;
        existingCar.Model = muscleCar.Model;
        existingCar.Year = muscleCar.Year;
        existingCar.Weight = muscleCar.Weight;
        existingCar.QuarterMileTime = muscleCar.QuarterMileTime;
        existingCar.QuarterMileSpeed = muscleCar.QuarterMileSpeed;

        return Task.FromResult<MuscleCar?>(existingCar);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var car = _muscleCars.FirstOrDefault(c => c.Id == id);
        if (car == null)
            return Task.FromResult(false);

        _muscleCars.Remove(car);
        return Task.FromResult(true);
    }
}
