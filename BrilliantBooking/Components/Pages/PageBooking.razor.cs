using Data.VO.Booking;
using Data.VO.Operations;
using Data.VO.Restaurant;
using FDM;
using Microsoft.AspNetCore.Components;

namespace BrilliantBooking.Components.Pages;

public partial class PageBooking
{
    private BookingVO Booking = new();
    private BookingFDM _fdm = new();


    [Parameter] public string? Restaurant { get; set; }

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

        }
    }

    private async Task beginEdit()
    {
        RestaurantVO restaurant;
        if (!Guid.TryParse(Restaurant , out Guid restaurantId))
        {
            throw new ArgumentException($"Invalid restaurant id: '{Restaurant}'.");
        }
        restaurant = new RestaurantVO { Id = restaurantId };
        Booking = await _fdm.GetNew(restaurant);
    }


    private void GoToBookingStep(int step)
    {
        Booking.Step = step;
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

        Booking.Table = table;
        Booking.TableName = table.Name;
        StateHasChanged();
    }

    private async Task ConfirmBooking()
    {
        try
        {
            string s = await _fdm.Persist(Booking);
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