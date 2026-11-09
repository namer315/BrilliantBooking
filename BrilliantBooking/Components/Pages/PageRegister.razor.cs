using Data.VO.Restaurant;
using FDM;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace BrilliantBooking.Components.Pages;

public partial class PageRegister
{
    protected RestaurantVO Restaurant = new();
    private RestaurantFDM _fdm = new();

    protected const string InputClass = "w-full rounded-xl border border-stone-300 dark:border-stone-700 bg-white dark:bg-stone-800 px-4 py-2.5 text-sm text-stone-900 dark:text-stone-100 placeholder:text-stone-400 focus:outline-none focus-visible:ring-2 focus-visible:ring-apple-blue";

    protected static readonly string[] StepTitles = ["Basics", "Location", "Hours", "Capacity", "Owner", "Review"];
    protected static readonly string[] CuisineOptions = ["Seafood", "French", "Japanese", "Italian", "Farm-to-Table"];
    protected Dictionary<string, string> Errors { get; } = new();
    protected List<string> SubmitErrors { get; } = new();
    protected int CurrentStep { get; set; }
    protected bool Submitted { get; set; }
    protected string SubmittedName { get; set; } = string.Empty;

    protected override void OnInitialized()
    {
        try
        {
            beginEdit();
        }
        catch(Exception ex)
        {
            ShowException(ex);
        }
    }

    private async Task beginEdit()
    {
        Restaurant = await _fdm.GetNew();
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

    protected async Task OnPhotosSelected(InputFileChangeEventArgs e)
    {
        //Restaurant.Photos = e.GetMultipleFiles(5)
        //    .Select(file => new RestaurantPhotoVO { 
        //        Name = file.Name,
        //        Photo = file., 
        //        Restaurant = Restaurant}).ToList();

        IList<RestaurantPhotoVO> photoList = new List<RestaurantPhotoVO>();

        foreach (var file in e.GetMultipleFiles(5))
        {
            using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
            using var memory = new MemoryStream();
            await stream.CopyToAsync(memory);

            Restaurant.PhotoList.Add(new RestaurantPhotoVO
            {
                Name = file.Name ,
                Photo = memory.ToArray() ,
                Restaurant = Restaurant
            });
        }
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
        try
        {
            SubmitErrors.Clear();
            await _fdm.Persist(Restaurant);
            SubmittedName = Restaurant.Name;
            Submitted = true;
        }
        catch (Exception ex)
        {

        }
    }

    private bool ValidateStep(int step)
    {
        Errors.Clear();

        switch (step)
        {
            case 0:
                ValidateMember(Restaurant, nameof(Restaurant.Name));
                ValidateMember(Restaurant, nameof(Restaurant.Cuisine));
                ValidateMember(Restaurant, nameof(Restaurant.Description));
                break;
            case 1:
                ValidateMember(Restaurant, nameof(Restaurant.Address));
                ValidateMember(Restaurant, nameof(Restaurant.Phone));
                ValidateMember(Restaurant, nameof(Restaurant.Email));
                break;
            case 2:
                foreach (var hour in Restaurant.OpeningHourList.Where(h => !h.IsClosed))
                {
                    if (hour.OpenTime is null || hour.CloseTime is null || hour.OpenTime >= hour.CloseTime)
                    {
                        Errors[$"hours-{hour.Day}"] = "Opening time must be before closing time.";
                    }
                }
                break;
            case 3:
                ValidateMember(Restaurant, nameof(Restaurant.TableCapacity));
                break;
            case 4:
                ValidateMember(Restaurant.Owner, nameof(Restaurant.Owner.FullName));
                ValidateMember(Restaurant.Owner, nameof(Restaurant.Owner.Email));
                ValidateMember(Restaurant.Owner, nameof(Restaurant.Owner.Phone));
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
