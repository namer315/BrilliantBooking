using System.Globalization;
using ERPBooking.Booking;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ERPBooking.Pages;

public sealed class IndexModel(
    RestaurantCatalog catalog,
    ReservationStore reservationStore) : PageModel
{
    public string ApplicationName => "Cape & Table";
    public string PageTitle => "Find your table in Cape Town";
    public string? Query { get; private set; }
    public string? Area { get; private set; }
    public string? Cuisine { get; private set; }
    public string Time { get; private set; } = string.Empty;
    public int PriceLevel { get; private set; }
    public DateOnly Date { get; private set; } = CapeTownClock.Today;
    public int Guests { get; private set; } = 2;
    public IReadOnlyList<string> Areas { get; private set; } = [];
    public IReadOnlyList<string> Cuisines { get; private set; } = [];
    public IReadOnlyList<RestaurantListing> Restaurants { get; private set; } = [];

    public async Task OnGetAsync(
        string? query,
        string? area,
        string? cuisine,
        int priceLevel,
        DateOnly? date,
        int guests,
        string? time,
        CancellationToken cancellationToken)
    {
        Query = query?.Trim();
        Area = area;
        Cuisine = cuisine;
        PriceLevel = Math.Clamp(priceLevel, 0, 4);
        Date = date ?? CapeTownClock.Today;
        Guests = Math.Clamp(guests == 0 ? 2 : guests, 1, 8);
        Time = TimeOnly.TryParseExact(time, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _)
            ? time!
            : string.Empty;
        Areas = catalog.GetAll().Select(restaurant => restaurant.Area).Distinct().Order().ToArray();
        Cuisines = catalog.GetAll().Select(restaurant => restaurant.Cuisine).Distinct().Order().ToArray();

        var reservations = await reservationStore.GetAllAsync(cancellationToken);
        var today = CapeTownClock.Today;
        var currentTime = CapeTownClock.CurrentTime;
        var selectedTime = TimeOnly.TryParseExact(Time, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedTime)
            ? parsedTime
            : (TimeOnly?)null;

        Restaurants = catalog.GetAll()
            .Where(restaurant =>
                (string.IsNullOrWhiteSpace(Query) ||
                 Contains(restaurant.Name, Query) ||
                 Contains(restaurant.Cuisine, Query) ||
                 Contains(restaurant.Area, Query) ||
                 Contains(restaurant.Description, Query)) &&
                (string.IsNullOrWhiteSpace(Area) || restaurant.Area == Area) &&
                (string.IsNullOrWhiteSpace(Cuisine) || restaurant.Cuisine == Cuisine) &&
                (PriceLevel == 0 || restaurant.PriceLevel <= PriceLevel))
            .Select(restaurant => new RestaurantListing(
                restaurant,
                restaurant.GetSlots(Date, today)
                    .Where(slot =>
                        (Date != today || slot > currentTime) &&
                        (selectedTime is null || slot == selectedTime) &&
                        reservations
                            .Where(reservation =>
                                reservation.RestaurantSlug == restaurant.Slug &&
                                reservation.Date == Date &&
                                reservation.Time == slot)
                            .Sum(reservation => reservation.Guests) + Guests <= restaurant.CoversPerSlot)
                    .Take(4)
                    .ToArray()))
            .Where(listing => listing.AvailableTimes.Count > 0)
            .ToArray();
    }

    private static bool Contains(string source, string value) =>
        source.Contains(value, StringComparison.OrdinalIgnoreCase);
}

public sealed record RestaurantListing(Restaurant Restaurant, IReadOnlyList<TimeOnly> AvailableTimes);
