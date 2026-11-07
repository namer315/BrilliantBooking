using Data.VO.Booking;
using FDM;

namespace BrilliantBooking.Components.Pages;

public partial class PageBooking
{
    private BookingVO Booking;
    private BookingFDM _fdm = new();

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


    private void ConfirmBooking()
    {

    }
    private void Save()
    {

    }

    private void ViewMyBookings()
    {

    }
}