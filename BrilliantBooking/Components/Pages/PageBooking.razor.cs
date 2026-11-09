using Data.VO.Booking;
using Data.VO.Operations;
using FDM;
using Microsoft.AspNetCore.Components;

namespace BrilliantBooking.Components.Pages;

public partial class PageBooking
{
    private BookingVO Booking;
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
        Booking = await _fdm.GetNew();
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
            var s = Math.Max(t.Seats, 1);
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

    private void SelectTable(TableVO table)
    {
        if (table.Seats < Booking.GuestCount)
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