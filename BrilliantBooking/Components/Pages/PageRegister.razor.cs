using Data.VO.Restaurant;
using FDM;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace BrilliantBooking.Components.Pages;

public partial class AddBase : ComponentBase
{
    private RestaurantFDM _fdm = new();

    protected const string InputClass = "w-full rounded-xl border border-stone-300 dark:border-stone-700 bg-white dark:bg-stone-800 px-4 py-2.5 text-sm text-stone-900 dark:text-stone-100 placeholder:text-stone-400 focus:outline-none focus-visible:ring-2 focus-visible:ring-apple-blue";

    protected static readonly string[] StepTitles = ["Basics", "Location", "Hours", "Capacity", "Owner", "Review"];
    protected static readonly string[] CuisineOptions = ["Seafood", "French", "Japanese", "Italian", "Farm-to-Table"];

    protected RestaurantVO Model { get; } = new();
    protected Dictionary<string, string> Errors { get; } = new();
    protected List<string> SubmitErrors { get; } = new();
    protected int CurrentStep { get; set; }
    protected bool Submitted { get; set; }
    protected string SubmittedName { get; set; } = string.Empty;

    protected override void OnInitialized()
    {
        Model.Owner = new RestaurantOwnerVO();
        Model.OpeningHours = new[]
            {
                DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday,
                DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday
            }
            .Select(day => new RestaurantHourVO
            {
                Day = day,
                Open = new TimeOnly(12, 0),
                Close = new TimeOnly(22, 0)
            })
            .ToList();
    }

    protected string? ErrorText(string key) => Errors.GetValueOrDefault(key);

    protected void Next()
    {
        if (ValidateStep(CurrentStep))
        {
            CurrentStep++;
        }
    }

    protected void Back()
    {
        Errors.Clear();
        if (CurrentStep > 0)
        {
            CurrentStep--;
        }
    }

    protected void GoTo(int step)
    {
        if (step < CurrentStep)
        {
            Errors.Clear();
            CurrentStep = step;
        }
    }

    protected void OnPhotosSelected(InputFileChangeEventArgs e)
    {
        Model.Photos = e.GetMultipleFiles(5).Select(file => new RestaurantPhotoVO { FileName = file.Name }).ToList();
    }

    protected async Task HandleFormSubmit()
    {
        if (CurrentStep == StepTitles.Length - 1)
        {
            await Submit();
        }
        else
        {
            Next();
        }
    }

    protected static TimeOnly? ParseTime(object? value) =>
        TimeOnly.TryParse(value?.ToString(), out var time) ? time : null;

    private async Task Submit()
    {
        SubmitErrors.Clear();
        await _fdm.Persist(Model);
        SubmittedName = Model.Name;
        Submitted = true;
    }

    private bool ValidateStep(int step)
    {
        Errors.Clear();

        switch (step)
        {
            case 0:
                ValidateMember(Model, nameof(Model.Name));
                ValidateMember(Model, nameof(Model.Cuisine));
                ValidateMember(Model, nameof(Model.Description));
                break;
            case 1:
                ValidateMember(Model, nameof(Model.Address));
                ValidateMember(Model, nameof(Model.Phone));
                ValidateMember(Model, nameof(Model.Email));
                break;
            case 2:
                foreach (var hour in Model.OpeningHours.Where(h => !h.IsClosed))
                {
                    if (hour.Open is null || hour.Close is null || hour.Open >= hour.Close)
                    {
                        Errors[$"hours-{hour.Day}"] = "Opening time must be before closing time.";
                    }
                }
                break;
            case 3:
                ValidateMember(Model, nameof(Model.TableCapacity));
                break;
            case 4:
                ValidateMember(Model.Owner, nameof(Model.Owner.FullName));
                ValidateMember(Model.Owner, nameof(Model.Owner.Email));
                ValidateMember(Model.Owner, nameof(Model.Owner.Phone));
                break;
        }

        return Errors.Count == 0;
    }

    private void ValidateMember(object instance, string member)
    {
        var value = instance.GetType().GetProperty(member)!.GetValue(instance);
        var context = new ValidationContext(instance) { MemberName = member };
        var results = new List<ValidationResult>();

        if (!Validator.TryValidateProperty(value, context, results))
        {
            Errors[member] = results[0].ErrorMessage ?? "Invalid value.";
        }
    }
}
