using BrilliantBooking.Extensions;
using BrilliantBooking.Services;
using Data.VO.Booking;
using Data.VO.Operations;
using FDM;
using Microsoft.AspNetCore.Components;
using System.Globalization;
using static Data.VO.MenuVO;

namespace BrilliantBooking.Components.Pages;

[NavigationRoute(NavigationRoutes.Booking)]
[RouteAccess]
public partial class PageBooking
{
    private BookingVO Booking = new();
    private BookingFDM _fdm = new();

    #region SupplyParameterFromQuery
    private Guid? restaurantId;
    [SupplyParameterFromQuery]
    public string? qR { set => value.ValidateQueryGuidParameter(NavigationManager , "Restaurant" , guid => restaurantId = guid); }

    #endregion


    public IReadOnlyList<(int Offset , string Label)> DateShortcuts { get; } = new[]
    {
        (0, "Today"), (1, "Tomorrow"), (2, "This Friday")
    };
    public IReadOnlyList<string> Occasions { get; } = new[]
    {
        "🎂 Birthday", "🥂 Anniversary", "💼 Business Meal", "❤️ Date Night"
    };
    public IReadOnlyList<string> DietaryOptions { get; } = new[]
    {
        "🌱 Vegan / Plant", "🌾 Gluten-Free", "🌙 Halal", "🥜 Nut Allergy", "🥛 Dairy Free"
    };

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await beginEdit();
        }
        catch(Exception ex)
        {
            ShowException(ex);
        }
    }

    private async Task beginEdit()
    {
        if (GetEntityId() is Guid id && id != Guid.Empty)
        {
            Booking = await _fdm.GetBookingBy(id).ShowProcessing(this);
        }
        else
        {
            Booking = await _fdm.GetNew(restaurantId.Value).ShowProcessing(this);
        }
    }


    private void GoToBookingStep(int step)
    {
        Booking.Step = step;
    }

    private void SelectDateShortcut(int offset)
    {
        Booking.Date = GetShortcutDate(offset);
    }

    private void OnDateChanged(ChangeEventArgs e)
    {
        if (DateOnly.TryParseExact(e.Value?.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            Booking.Date = date;
        }
    }

    private DateOnly GetShortcutDate(int offset)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3));

        return offset switch
        {
            0 => today,
            1 => today.AddDays(1),
            2 => today.AddDays(((int)DayOfWeek.Friday - (int)today.DayOfWeek + 7) % 7),
            _ => today.AddDays(offset)
        };
    }

    private void SubmitGuestDetails()
    {
        GoToBookingStep(4);
    }
    public void ToggleOccasion(string occasion) => Booking.Occasion = Booking.Occasion == occasion ? "" : occasion;
    public void ToggleDietary(string item)
    {
        if (!Booking.Dietary.Add(item)) Booking.Dietary.Remove(item);
    }

    public (List<(TableVO Table, double X, double Y, double Width)> Layout, double CanvasHeight) BuildFloorLayout(IEnumerable<TableVO> tables)
    {
        var layout = new List<(TableVO Table, double X, double Y, double Width)>();
        double x = 30, y = 40, rowHeight = 0;
        foreach (var t in tables)
        {
            var s = Math.Max(t.GuestCount, 1);
            double w = 60 + s * 10;
            var h = s <= 2 ? 44 + s * 6 : 70;
            if (x + w > 770)
            {
                x = 30;
                y += rowHeight + 30;
                rowHeight = 0;
            }
            layout.Add((t, x + w / 2, y + h / 2, w));
            x += w + 10 + s * 4;
            rowHeight = Math.Max(rowHeight, h);
        }
        return (layout, Math.Max(450, y + rowHeight + 40));
    }

    public int SelectedFloorIndex { get; set; }

    protected IList<FloorVO> Floors => Booking.Restaurant?.FloorList ?? new List<FloorVO>();

    protected IList<TableVO> SelectedFloorTables =>
        Floors.Count == 0
            ? new List<TableVO>()
            : Floors[Math.Clamp(SelectedFloorIndex, 0, Floors.Count - 1)].TableList;

    protected void SelectFloor(int index)
    {
        if (index < 0 || index >= Floors.Count)
        {
            return;
        }

        SelectedFloorIndex = index;
    }

    protected bool CanSelectTable(TableVO table) =>
        table.Status != TableVO.TableStatus.Reserved && table.GuestCount >= Booking.GuestCount;

    public static string StatusColor(TableVO.TableStatus status) => status switch
    {
        TableVO.TableStatus.Available => "#34C759",
        TableVO.TableStatus.Selected => "#0A84FF",
        TableVO.TableStatus.Reserved => "#FF9500",
        _ => "#9CA3AF"
    };

    public static string StatusLabel(TableVO.TableStatus status) => status switch
    {
        TableVO.TableStatus.Available => "Available",
        TableVO.TableStatus.Selected => "Selected",
        TableVO.TableStatus.Reserved => "Reserved",
        _ => "Unknown"
    };

    private void SelectTable(TableVO table)
    {
        if (!CanSelectTable(table))
        {
            return;
        }

        Booking.BookingTable = table;
        StateHasChanged();
    }

    private async Task ConfirmBooking()
    {
        try
        {
            Booking = await _fdm.Persist(Booking).ShowProcessing(this);
        }
        catch(Exception ex)
        {

        }
    }
    private void Save()
    {

    }

    private void ViewMyBookings()
    {

    }
}