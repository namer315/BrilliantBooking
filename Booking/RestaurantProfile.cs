namespace ERPBooking.Booking;

public sealed record RestaurantProfile(
    Restaurant Restaurant,
    string Email,
    string Website,
    IReadOnlyList<RestaurantProfilePhoto> Photos,
    IReadOnlyList<RestaurantMenuCategory> MenuCategories,
    IReadOnlyList<RestaurantOpeningHour> OpeningHours,
    IReadOnlyList<RestaurantClosure> ClosureDates,
    IReadOnlyList<RestaurantProfileEvent> Events,
    IReadOnlyList<RestaurantReview> Reviews,
    IReadOnlyList<RestaurantFacility> Facilities,
    IReadOnlyList<RestaurantPromotion> Promotions)
{
    public decimal AverageRating => Reviews.Count == 0
        ? Restaurant.Rating
        : Math.Round(Reviews.Average(review => (decimal)review.Rating), 1);

    public int ReviewCount => Reviews.Count == 0 ? Restaurant.ReviewCount : Reviews.Count;
}

public sealed record RestaurantProfilePhoto(string Category, string Url, string AltText);

public sealed record RestaurantMenuCategory(string Name, string Description, IReadOnlyList<RestaurantMenuItem> Items);

public sealed record RestaurantMenuItem(string Name, string Description, decimal Price);

public sealed record RestaurantOpeningHour(DayOfWeek Day, TimeOnly OpensAt, TimeOnly ClosesAt);

public sealed record RestaurantClosure(DateOnly Date, string Reason);

public sealed record RestaurantProfileEvent(string Title, string Description, DateTime StartsAt, DateTime EndsAt);

public sealed record RestaurantReview(string Author, int Rating, string Comment, DateTime CreatedAt);

public sealed record RestaurantFacility(string Code, string Label, string Description);

public sealed record RestaurantPromotion(string Type, string Title, string Body, DateOnly? ValidFrom, DateOnly? ValidTo);
