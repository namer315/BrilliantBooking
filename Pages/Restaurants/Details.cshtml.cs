using System.ComponentModel.DataAnnotations;
using System.Globalization;
using ERPBooking.Booking;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ERPBooking.Pages.Restaurants;

public sealed class DetailsModel(
    RestaurantCatalog catalog,
    ReservationStore reservationStore) : PageModel
{
    [BindProperty]
    public BookingInput Input { get; set; } = new();

    public Restaurant? Restaurant { get; private set; }
    public IReadOnlyList<TimeOnly> AvailableTimes { get; private set; } = [];
    public string? ConfirmationReference { get; private set; }

    public async Task<IActionResult> OnGetAsync(
        string slug,
        string? date,
        int guests,
        string? time,
        CancellationToken cancellationToken)
    {
        var selectedDate = DateOnly.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate)
            ? parsedDate
            : CapeTownClock.Today;
        Input.Date = selectedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        Input.Guests = Math.Clamp(guests == 0 ? 2 : guests, 1, 8);
        Input.Time = TimeOnly.TryParseExact(time, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedTime)
            ? RestaurantCatalog.FormatTime(parsedTime)
            : string.Empty;

        return await LoadRestaurantAsync(slug, cancellationToken) ? Page() : NotFound();
    }

    public async Task<IActionResult> OnPostAsync(string slug, CancellationToken cancellationToken)
    {
        var restaurant = catalog.Find(slug);
        if (restaurant is null)
        {
            return NotFound();
        }

        if (!DateOnly.TryParseExact(Input.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            ModelState.AddModelError("Input.Date", "Choose a valid reservation date.");
        }

        if (!TimeOnly.TryParseExact(Input.Time, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
        {
            ModelState.AddModelError("Input.Time", "Choose an available reservation time.");
        }

        if (ModelState.IsValid)
        {
            var reservation = new Reservation(
                $"CT-{Guid.NewGuid():N}"[..11].ToUpperInvariant(),
                restaurant.Slug,
                Input.GuestName.Trim(),
                Input.Email.Trim(),
                Input.Phone.Trim(),
                date,
                time,
                Input.Guests,
                DateTime.UtcNow);

            var result = await reservationStore.TryCreateAsync(reservation, cancellationToken);
            if (result == ReservationResult.Confirmed)
            {
                ConfirmationReference = reservation.Reference;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "That time is no longer available. Please choose another table time.");
            }
        }

        await LoadRestaurantAsync(slug, cancellationToken);
        return Page();
    }

    private async Task<bool> LoadRestaurantAsync(string slug, CancellationToken cancellationToken)
    {
        Restaurant = catalog.Find(slug);
        if (Restaurant is null)
        {
            return false;
        }

        var date = DateOnly.TryParseExact(Input.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var selectedDate)
            ? selectedDate
            : CapeTownClock.Today;
        var reservations = await reservationStore.GetAllAsync(cancellationToken);
        var today = CapeTownClock.Today;
        var currentTime = CapeTownClock.CurrentTime;
        AvailableTimes = Restaurant.GetSlots(date, today)
            .Where(slot =>
                (date != today || slot > currentTime) &&
                reservations
                    .Where(reservation =>
                        reservation.RestaurantSlug == Restaurant.Slug &&
                        reservation.Date == date &&
                        reservation.Time == slot)
                    .Sum(reservation => reservation.Guests) + Math.Clamp(Input.Guests, 1, 8) <= Restaurant.CoversPerSlot)
            .ToArray();
        return true;
    }
}

public sealed class BookingInput
{
    [Required, StringLength(100)]
    [Display(Name = "Your name")]
    public string GuestName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(254)]
    [Display(Name = "Email address")]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(40)]
    [Display(Name = "Phone number")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Date")]
    public string Date { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Time")]
    public string Time { get; set; } = string.Empty;

    [Range(1, 8)]
    [Display(Name = "Party size")]
    public int Guests { get; set; } = 2;
}
