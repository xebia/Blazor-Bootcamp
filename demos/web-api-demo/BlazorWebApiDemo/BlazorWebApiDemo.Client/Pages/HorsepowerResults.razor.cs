using BlazorWebApiDemo.Client.Models;
using BlazorWebApiDemo.Client.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorWebApiDemo.Client.Pages;

public partial class HorsepowerResults
{
    [Inject]
    private MuscleCarApiService MuscleCarApi { get; set; } = default!;

    private List<MuscleCar> muscleCars = new();
    private bool isLoading = true;
    
    // Form state
    private bool showForm = false;
    private bool isEditing = false;
    private MuscleCar editingCar = new();
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        isLoading = true;
        errorMessage = null;
        try
        {
            muscleCars = await MuscleCarApi.GetAllAsync();
            muscleCars = muscleCars.OrderByDescending(c => c.EstimatedHorsepower).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading muscle car data: {ex.Message}");
            errorMessage = "Failed to load data.";
            muscleCars = new List<MuscleCar>();
        }
        finally
        {
            isLoading = false;
        }
    }

    private void ShowAddForm()
    {
        editingCar = new MuscleCar
        {
            Year = 1970,
            Weight = 3500,
            QuarterMileTime = 14.0,
            QuarterMileSpeed = 100.0
        };
        isEditing = false;
        showForm = true;
        errorMessage = null;
    }

    private void ShowEditForm(MuscleCar car)
    {
        editingCar = new MuscleCar
        {
            Id = car.Id,
            Make = car.Make,
            Model = car.Model,
            Year = car.Year,
            Weight = car.Weight,
            QuarterMileTime = car.QuarterMileTime,
            QuarterMileSpeed = car.QuarterMileSpeed
        };
        isEditing = true;
        showForm = true;
        errorMessage = null;
    }

    private void CancelForm()
    {
        showForm = false;
        editingCar = new();
        errorMessage = null;
    }

    private async Task SaveAsync()
    {
        errorMessage = null;
        
        try
        {
            if (isEditing)
            {
                var updated = await MuscleCarApi.UpdateAsync(editingCar.Id, editingCar);
                if (updated == null)
                {
                    errorMessage = "Failed to update muscle car.";
                    return;
                }
            }
            else
            {
                var created = await MuscleCarApi.CreateAsync(editingCar);
                if (created == null)
                {
                    errorMessage = "Failed to create muscle car.";
                    return;
                }
            }

            showForm = false;
            editingCar = new();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving: {ex.Message}");
            errorMessage = "An error occurred while saving.";
        }
    }

    private async Task DeleteAsync(MuscleCar car)
    {
        errorMessage = null;
        
        try
        {
            var success = await MuscleCarApi.DeleteAsync(car.Id);
            if (success)
            {
                await LoadDataAsync();
            }
            else
            {
                errorMessage = "Failed to delete muscle car.";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting: {ex.Message}");
            errorMessage = "An error occurred while deleting.";
        }
    }
}
