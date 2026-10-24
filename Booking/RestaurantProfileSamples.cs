namespace ERPBooking.Booking;

internal static class RestaurantProfileSamples
{
    public static RestaurantProfile Create(Restaurant restaurant)
    {
        if (!string.Equals(restaurant.Slug, "salt-and-sea", StringComparison.OrdinalIgnoreCase))
        {
            return new RestaurantProfile(
                restaurant,
                string.Empty,
                string.Empty,
                [new RestaurantProfilePhoto("Dining", restaurant.ImageUrl, restaurant.ImageAlt)],
                [],
                restaurant.OpenDays.Select(day => new RestaurantOpeningHour(day, restaurant.OpensAt, restaurant.ClosesAt)).ToArray(),
                [],
                [],
                [],
                [],
                []);
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        var eventStart = DateTime.Today.AddDays(14).AddHours(18);
        return new RestaurantProfile(
            restaurant,
            "bookings@saltandsea.example",
            "https://saltandsea.example",
            [
                new RestaurantProfilePhoto("Dining", restaurant.ImageUrl, restaurant.ImageAlt),
                new RestaurantProfilePhoto("Food", "https://images.unsplash.com/photo-1547592180-85f173990554?auto=format&fit=crop&w=900&q=85", "Fresh seafood dishes served to share"),
                new RestaurantProfilePhoto("Dining", "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?auto=format&fit=crop&w=900&q=85", "Warm and welcoming restaurant dining area"),
                new RestaurantProfilePhoto("Ambience", "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?auto=format&fit=crop&w=900&q=85", "Candlelit restaurant setting"),
            ],
            [
                new RestaurantMenuCategory("To start", "Small plates inspired by the Cape coast.",
                [
                    new RestaurantMenuItem("West Coast mussels", "White wine, garlic, herbs, and toasted sourdough.", 155m),
                    new RestaurantMenuItem("Crispy calamari", "Lemon aioli and a fresh herb salad.", 135m),
                ]),
                new RestaurantMenuCategory("From the sea", "Seasonal seafood, simply prepared.",
                [
                    new RestaurantMenuItem("Line-caught catch", "Grilled fish, seasonal greens, and beurre blanc.", 285m),
                    new RestaurantMenuItem("Seafood linguine", "Prawns, mussels, tomato, chilli, and parsley.", 245m),
                ]),
                new RestaurantMenuCategory("Something sweet", "A little finish for the table.",
                [
                    new RestaurantMenuItem("Malva pudding", "Warm Cape classic with vanilla custard.", 95m),
                    new RestaurantMenuItem("Seasonal fruit sorbet", "A rotating selection made in-house.", 75m),
                ]),
            ],
            restaurant.OpenDays.Select(day => new RestaurantOpeningHour(day, restaurant.OpensAt, restaurant.ClosesAt)).ToArray(),
            [new RestaurantClosure(today.AddDays(40), "Closed for a private event.")],
            [new RestaurantProfileEvent("Sunset seafood supper", "Join us for a shared seafood menu and a welcome glass of local sparkling wine.", eventStart, eventStart.AddHours(3))],
            [
                new RestaurantReview("Mia L.", 5, "Beautifully fresh seafood and a relaxed table overlooking the evening light.", DateTime.Today.AddDays(-3)),
                new RestaurantReview("Daniel R.", 5, "The mussels and line-caught fish were excellent. Friendly, attentive service.", DateTime.Today.AddDays(-12)),
                new RestaurantReview("Ayesha K.", 4, "A lovely spot for a long dinner. The seasonal menu had something for everyone.", DateTime.Today.AddDays(-21)),
            ],
            [
                new RestaurantFacility("parking", "Nearby parking", "Public parking is available along Beach Road."),
                new RestaurantFacility("outdoor-seating", "Outdoor seating", "Ask for a terrace table when booking, subject to availability."),
                new RestaurantFacility("wheelchair-access", "Wheelchair access", "Step-free access to the main dining area."),
                new RestaurantFacility("family-friendly", "Family friendly", "High chairs are available on request."),
            ],
            [new RestaurantPromotion("Special menu", "Coastal catch menu", "A three-course set menu featuring the catch of the day, available for a limited time.", today, today.AddDays(45))]);
    }
}
