using BrilliantBooking.Extensions;
using Data.DTO;
using Data.VO.Operations;
using Data.VO.Restaurant;
using FDM;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace BrilliantBooking.Components.Pages;

public partial class PageRestaurant
{
    private RestaurantVO Restaurant = new RestaurantVO();
    private RestaurantFDM _fdm = new RestaurantFDM();

    protected const string InputClass = "w-full rounded-xl border border-white/60 dark:border-white/10 bg-white/60 dark:bg-stone-900/60 backdrop-blur-md px-4 py-2.5 text-sm text-stone-900 dark:text-stone-100 placeholder:text-stone-400 focus:outline-none focus-visible:ring-2 focus-visible:ring-apple-blue";

    protected static readonly string[] StepTitles = ["Basics" , "Location" , "Hours" , "Photos" , "Tables" , "Review"];
    protected static readonly string[] CuisineOptions = ["Seafood" , "French" , "Japanese" , "Italian" , "Farm-to-Table"];
    protected Dictionary<string , string> Errors { get; } = new();
    protected List<string> SubmitErrors { get; } = new();
    protected int CurrentStep { get; set; }
    protected bool Submitted { get; set; }
    protected string SubmittedName { get; set; } = string.Empty;
    protected int SelectedPhotoIndex { get; set; }
    protected TableVO NewTable { get; set; } = new();
    protected int EditingTableIndex { get; set; } = -1;
    protected bool IsEditingTable => EditingTableIndex >= 0;

    protected override void OnInitialized()
    {
        try
        {
            beginEdit();
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    private async Task beginEdit()
    {
        if (GetEntityId() is Guid id && id != Guid.Empty)
        {
            Restaurant = await _fdm.GetRestaurantBy(id).ShowProcessing(this);
        }
        else
            Restaurant = await _fdm.GetNew().ShowProcessing(this);
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
        if (step < CurrentStep || Restaurant.Id != Guid.Empty)
        {
            Errors.Clear();
            CurrentStep = step;
        }
    }

    protected async Task OnPhotosSelected(InputFileChangeEventArgs e)
    {
        IList<FileDTO> files = await e.ToFileDTOs(5);

        _fdm.AddPhotos(Restaurant , files);
        SelectedPhotoIndex = 0;
        StateHasChanged();
    }

    protected async Task HandleFormSubmit()
    {
        if (CurrentStep == StepTitles.Length - 1)
        {
            await endEdit();
        }
        else
        {
            Next();
        }
    }

    protected void SelectPhoto(int index) =>
        SelectedPhotoIndex = Math.Clamp(index , 0 , Math.Max(Restaurant.PhotoList.Count - 1 , 0));

    private bool IsMain(RestaurantPhotoVO photo) => photo.Sequence == 1;

    private void SetMainPhoto(int index)
    {
        if (index < 0 || index >= Restaurant.PhotoList.Count)
        {
            return;
        }

        var photo = Restaurant.PhotoList[index];
        Restaurant.PhotoList.RemoveAt(index);
        Restaurant.PhotoList.Insert(0, photo);
        _fdm.RebindPhotoSequence(Restaurant);
        SelectedPhotoIndex = 0;
    }

    #region Table tab
    protected void AddTable()
    {
        Errors.Clear();
        if (!ValidateTable(NewTable))
        {
            return;
        }

        Restaurant.TableList.Add(new TableVO
        {
            Name = NewTable.Name.Trim(),
            GuestCount = NewTable.GuestCount,
            Description = string.IsNullOrWhiteSpace(NewTable.Description) ? null : NewTable.Description.Trim()
        });
        NewTable = new TableVO();
    }

    protected void EditTable(int index)
    {
        if (index < 0 || index >= Restaurant.TableList.Count)
        {
            return;
        }

        var table = Restaurant.TableList[index];
        Errors.Clear();
        NewTable = new TableVO
        {
            Name = table.Name,
            GuestCount = table.GuestCount,
            Description = table.Description
        };
        EditingTableIndex = index;
    }

    protected void SaveTable()
    {
        if (!IsEditingTable)
        {
            AddTable();
            return;
        }

        Errors.Clear();
        if (!ValidateTable(NewTable))
        {
            return;
        }

        var table = Restaurant.TableList[EditingTableIndex];
        table.Name = NewTable.Name.Trim();
        table.GuestCount = NewTable.GuestCount;
        table.Description = string.IsNullOrWhiteSpace(NewTable.Description) ? null : NewTable.Description.Trim();
        CancelEdit();
    }

    protected void CancelEdit()
    {
        NewTable = new TableVO();
        EditingTableIndex = -1;
        foreach (var key in Errors.Keys.Where(k => k.StartsWith("table-")).ToList())
        {
            Errors.Remove(key);
        }
    }

    protected void RemoveTable(int index)
    {
        if (index < 0 || index >= Restaurant.TableList.Count || index == EditingTableIndex)
        {
            return;
        }

        Restaurant.TableList.RemoveAt(index);
        if (IsEditingTable && index < EditingTableIndex)
        {
            EditingTableIndex--;
        }
    }

    private bool ValidateTable(TableVO table)
    {
        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(table, new ValidationContext(table), results, validateAllProperties: true))
        {
            foreach (var result in results)
            {
                var member = result.MemberNames.FirstOrDefault() ?? string.Empty;
                Errors[$"table-{member}"] = result.ErrorMessage ?? "Invalid value.";
            }
        }

        return Errors.Count == 0;
    }
    #endregion

    protected static TimeOnly? ParseTime(object? value) =>
        TimeOnly.TryParse(value?.ToString() , out var time) ? time : null;

    private async Task endEdit()
    {
        try
        {
            SubmitErrors.Clear();
            Restaurant = await _fdm.Persist(Restaurant).ShowProcessing(this);
            SubmittedName = Restaurant.Name;
            Submitted = true;
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    private bool ValidateStep(int step)
    {
        Errors.Clear();

        switch (step)
        {
            case 0:
                ValidateMember(Restaurant , nameof(Restaurant.Name));
                ValidateMember(Restaurant , nameof(Restaurant.Cuisine));
                ValidateMember(Restaurant , nameof(Restaurant.Description));
                break;
            case 1:
                ValidateMember(Restaurant , nameof(Restaurant.Address));
                ValidateMember(Restaurant , nameof(Restaurant.Phone));
                ValidateMember(Restaurant , nameof(Restaurant.Email));
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
            case 4:
                ValidateMember(Restaurant , nameof(Restaurant.TableCapacity));
                if (Restaurant.TableList.Count == 0)
                {
                    Errors["table-list"] = "Add at least one table.";
                }
                break;
        }

        return Errors.Count == 0;
    }

    private void ValidateMember(object instance , string member)
    {
        var value = instance.GetType().GetProperty(member)!.GetValue(instance);
        var context = new ValidationContext(instance) { MemberName = member };
        var results = new List<ValidationResult>();

        if (!Validator.TryValidateProperty(value , context , results))
        {
            Errors[member] = results[0].ErrorMessage ?? "Invalid value.";
        }
    }
}