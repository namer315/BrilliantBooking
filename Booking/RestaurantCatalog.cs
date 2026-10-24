using System.Globalization;

namespace ERPBooking.Booking;

public sealed class RestaurantCatalog
{
    private static readonly IReadOnlyList<Restaurant> Restaurants =
    [
        new(
            "salt-and-sea", "Salt & Sea", "Seafood", "Sea Point", 3, 4.8m, 326,
            "Seasonal seafood, a lively open kitchen, and sunset views along the Atlantic seaboard.",
            "45 Beach Road, Sea Point, Cape Town", "+27 21 555 0101", "Daily, 12:00-22:00",
            "Line-caught fish, West Coast mussels, and a daily changing menu.",
            "https://images.unsplash.com/photo-1514933651103-005eec06c04b?auto=format&fit=crop&w=1000&q=85",
            "Warmly lit restaurant dining room", "Ocean views",
            new TimeOnly(17, 0), new TimeOnly(22, 0), 18,
            [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday]),
        new(
            "fynbos-table", "Fynbos Table", "South African", "Gardens", 3, 4.9m, 214,
            "A thoughtful celebration of local ingredients, Cape flavours, and the people who grow them.",
            "18 Kloof Street, Gardens, Cape Town", "+27 21 555 0102", "Tue-Sun, 17:30-21:30",
            "Seasonal tasting menu with vegetarian options; please advise us of dietary needs.",
            "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?auto=format&fit=crop&w=1000&q=85",
            "Elegant candlelit table setting", "Seasonal menu",
            new TimeOnly(17, 30), new TimeOnly(21, 30), 14,
            [DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday]),
        new(
            "little-lemongrass", "Little Lemongrass", "Thai", "City Bowl", 2, 4.6m, 188,
            "Bright, fragrant Thai cooking in a relaxed neighbourhood dining room.",
            "9 Bree Street, Cape Town City Centre", "+27 21 555 0103", "Daily, 12:00-22:00",
            "Curries, wok-fried favourites, and a generous range of vegetarian dishes.",
            "https://images.unsplash.com/photo-1552566626-52f8b828add9?auto=format&fit=crop&w=1000&q=85",
            "Modern restaurant interior with tables", "Vegetarian friendly",
            new TimeOnly(17, 0), new TimeOnly(22, 0), 20,
            [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday]),
        new(
            "olive-and-vine", "Olive & Vine", "Italian", "De Waterkant", 2, 4.7m, 267,
            "Handmade pasta, long lunches, and Italian wines served in a tucked-away courtyard.",
            "27 Dixon Street, De Waterkant, Cape Town", "+27 21 555 0104", "Daily, 12:00-22:00",
            "Fresh pasta, wood-fired sides, and a short menu made for sharing.",
            "https://images.unsplash.com/photo-1559339352-11d035aa65de?auto=format&fit=crop&w=1000&q=85",
            "Inviting restaurant interior", "Courtyard dining",
            new TimeOnly(17, 0), new TimeOnly(22, 0), 16,
            [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday]),
        new(
            "the-green-room", "The Green Room", "Vegetarian", "Woodstock", 2, 4.5m, 143,
            "Vegetable-forward plates, natural wines, and an easy-going creative district atmosphere.",
            "81 Albert Road, Woodstock, Cape Town", "+27 21 555 0105", "Wed-Mon, 17:00-21:30",
            "Plant-led small plates, seasonal mains, and vegan-friendly options.",
            "https://images.unsplash.com/photo-1544148103-0773bf10d330?auto=format&fit=crop&w=1000&q=85",
            "Colourful food on a restaurant table", "Plant-led",
            new TimeOnly(17, 0), new TimeOnly(21, 30), 12,
            [DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday]),
        new(
            "mountain-house", "Mountain House", "Modern European", "Constantia", 4, 4.9m, 302,
            "A destination dining room for unhurried dinners, seasonal produce, and local wine.",
            "6 Hohenort Avenue, Constantia, Cape Town", "+27 21 555 0106", "Tue-Sun, 18:00-21:30",
            "A seasonal set menu celebrating the Cape's farms and vineyards.",
            "https://images.unsplash.com/photo-1515003197210-e0cd7184f8b5?auto=format&fit=crop&w=1000&q=85",
            "A beautifully plated restaurant dish", "Special occasion",
            new TimeOnly(18, 0), new TimeOnly(21, 30), 10,
            [DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday])
    ];

    public IReadOnlyList<Restaurant> GetAll() => Restaurants;

    public Restaurant? Find(string slug) =>
        Restaurants.FirstOrDefault(restaurant =>
            string.Equals(restaurant.Slug, slug, StringComparison.OrdinalIgnoreCase));

    public static string FormatTime(TimeOnly time) =>
        time.ToString("HH:mm", CultureInfo.InvariantCulture);
}
