namespace BlazorWebApiDemo.Client.Models;

public class MuscleCar
{
    public int Id { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Weight { get; set; }
    public double QuarterMileTime { get; set; }
    public double QuarterMileSpeed { get; set; }
    public double EstimatedHorsepower { get; set; }
    public string FullName { get; set; } = string.Empty;
}
