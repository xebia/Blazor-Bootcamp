namespace AppServer.Models;

public class MuscleCar
{
    public int Id { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Weight { get; set; } // Weight in pounds
    public double QuarterMileTime { get; set; } // Time in seconds
    public double QuarterMileSpeed { get; set; } // Speed at end of quarter mile in MPH
    
    // Calculated property: Estimated horsepower using the trap speed method.
    // Formula: HP = Weight × (MPH / 234)³
    // This is the standard drag racing formula where 234 is a constant.
    // Example: 3500 lbs car with 105 mph trap speed = 3500 × (105/234)³ = 3500 × 0.0906 = 317 HP
    public double EstimatedHorsepower => Math.Round(Weight * Math.Pow(QuarterMileSpeed / 234.0, 3), 0);
    
    public string FullName => $"{Year} {Make} {Model}";
}
