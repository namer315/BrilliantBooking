using BrilliantBooking.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BrilliantBooking.Components.Pages;

public partial class HomeBase : ComponentBase, IDisposable
{
    [Inject] protected IJSRuntime JavaScript { get; set; } = null!;
    [Inject] public BookingUiStateService UiState { get; set; } = null!;

    public string View => UiState.View;
    public bool IsDark => UiState.IsDark;
    public bool MobileMenuOpen => UiState.MobileMenuOpen;
    public bool LocationModalOpen => UiState.LocationModalOpen;
    public string Location => UiState.Location;
    public IReadOnlyList<(string Value, string Label)> Locations { get; } = new[]
    {
        ("Cape Town, SA", "Cape Town City & Atlantic Seaboard"),
        ("Franschhoek, Winelands", "Franschhoek & Stellenbosch"),
        ("Constantia Valley", "Constantia Wine Route")
    };
    public IReadOnlyList<(string Value, string Label)> Cuisines { get; } = new[]
    {
        ("all", "All Cuisines"), ("Seafood", "Seafood"), ("French", "French"),
        ("Japanese", "Japanese Omakase"), ("Italian", "Artisanal Italian"), ("Farm-to-Table", "Farm-to-Table")
    };
    public IReadOnlyList<(string Value, string Label)> Prices { get; } = new[]
    {
        ("all", "All Prices"), ("$$", "$$ (Moderate)"), ("$$$", "$$$ (Fine)"), ("$$$$", "$$$$ (Luxury)")
    };
    public IReadOnlyList<(string Value, string Label)> Categories { get; } = new[]
    {
        ("all", "All Venues"), ("Seafood", "Waterfront & Seafood"), ("French", "Michelin & Fine Dining"),
        ("Japanese", "Japanese Omakase"), ("Italian", "Terrace Italian")
    };

    public string SearchText { get; set; } = "";
    public string CuisineFilter { get; set; } = "all";
    public string PriceFilter { get; set; } = "all";
    public bool FavoritesOnly => UiState.FavoritesOnly;
    public HashSet<int> Favorites => UiState.Favorites;
    public List<Restaurant> Restaurants { get; } = CreateRestaurants();

    public IReadOnlyList<Restaurant> FilteredRestaurants => Restaurants.Where(r =>
        (CuisineFilter == "all" || r.Cuisine == CuisineFilter) &&
        (PriceFilter == "all" || r.Price == PriceFilter) &&
        (!FavoritesOnly || Favorites.Contains(r.Id)) &&
        (string.IsNullOrWhiteSpace(SearchText) ||
            new[] { r.Name, r.Cuisine, r.Location, r.Description }.Any(v => v.Contains(SearchText, StringComparison.OrdinalIgnoreCase))))
        .ToList();

    public Restaurant? ActiveModalRestaurant { get; set; }
    public string ModalTab { get; set; } = "overview";
    public bool ShowReviewForm { get; set; }
    public string NewReviewName { get; set; } = "";
    public string NewReviewComment { get; set; } = "";
    public int NewReviewRating { get; set; } = 5;

    public Restaurant? BookingRestaurant { get; set; }
    public int BookingStep { get; set; } = 1;
    public int PartySize { get; set; } = 2;
    public int DateShortcut { get; set; } = 0;
    public string BookingDate { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");
    public string? TimeSlot { get; set; }
    public FloorTable? SelectedTable { get; set; }
    public string Occasion { get; set; } = "";
    public HashSet<string> Dietary { get; } = new();
    public string GuestName { get; set; } = "";
    public string GuestPhone { get; set; } = "";
    public string GuestEmail { get; set; } = "";
    public string GuestNotes { get; set; } = "";
    public bool BookingConfirmed { get; set; }
    public string ConfirmationCode { get; set; } = "";

    public IReadOnlyList<(int Offset, string Label)> DateShortcuts { get; } = new[]
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
    public IReadOnlyList<FloorTable> FloorTables { get; } = new[]
    {
        new FloorTable("Table 01", "Main Dining Room • Window View", 4),
        new FloorTable("Table 04", "Ocean Window Booth • VIP Preferred", 4),
        new FloorTable("Chef Counter 12", "Chef Counter • Front View Kitchen", 2),
        new FloorTable("Patio P1", "Outdoor Ocean Terrace • Heated", 2),
        new FloorTable("Patio P2", "Outdoor Ocean Terrace • Sun Deck", 4)
    };

    public List<Booking> Bookings { get; } = new()
    {
        new("#TBL-8942-CPT", 1, "Ocean Grill & Raw Bar", "Today", "19:30", 4, "Table 04 (Ocean Window)", "Active"),
        new("#TBL-2041-FRN", 2, "Le Petit Manoir", "14 Oct 2026", "20:00", 2, "Chef's Counter C1", "Past")
    };
    public string BookingsTab { get; set; } = "active";
    public string LookupCode { get; set; } = "";

    public List<Arrival> Arrivals { get; } = new()
    {
        new("19:30", "Vance Party (4)", "Table 04 • Gluten-Free", true),
        new("19:45", "Dr. A. Smith (2)", "Table 12 (Chef Counter) • VIP", false),
        new("20:15", "Claire Dupont (6)", "Patio Booth P1 • Birthday", false)
    };

    public IEnumerable<Booking> VisibleBookings => Bookings.Where(b =>
        BookingsTab == "active" ? b.Status == "Active" : b.Status == "Past");

    public bool IsSelectedTable(string name) => SelectedTable?.Name == name;

    public static string MapsUrl(Restaurant r) =>
        $"https://www.google.com/maps/search/?api=1&query={Uri.EscapeDataString($"{r.Name}, {r.Address}")}";

    public static string SelectedRestaurantCuisineLabel(Restaurant r) => r.Cuisine;

    protected override void OnInitialized()
    {
        UiState.Changed += OnStateChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JavaScript.InvokeVoidAsync("refreshReserveIcons");
    }

    private void OnStateChanged() => InvokeAsync(StateHasChanged);

    public void Dispose()
    {
        UiState.Changed -= OnStateChanged;
    }

    public async Task SwitchView(string view)
    {
        UiState.SetView(view);
        await JavaScript.InvokeVoidAsync("scrollTo", new { top = 0, behavior = "smooth" });
    }

    public void ToggleMobileMenu() => UiState.ToggleMobileMenu();

    public async Task ToggleTheme()
    {
        UiState.ToggleTheme();
        await JavaScript.InvokeVoidAsync("setDarkMode", IsDark);
    }

    public void ToggleFavoritesOnlyFilter() => UiState.ToggleFavoritesOnlyFilter();

    public void ToggleFavorite(Restaurant restaurant) => UiState.ToggleFavorite(restaurant.Id);

    public void SetCuisine(string value) => CuisineFilter = value;

    public void ResetFilters()
    {
        SearchText = "";
        CuisineFilter = "all";
        PriceFilter = "all";
        if (FavoritesOnly) UiState.ToggleFavoritesOnlyFilter();
    }

    public void ToggleLocationModal() => UiState.ToggleLocationModal();

    public void SetLocation(string value) => UiState.SetLocation(value);

    public void OpenRestaurant(Restaurant restaurant)
    {
        ActiveModalRestaurant = restaurant;
        ModalTab = "overview";
        ShowReviewForm = false;
        StateHasChanged();
    }

    public void CloseRestaurant()
    {
        ActiveModalRestaurant = null;
        StateHasChanged();
    }

    public void SwitchModalTab(string tab) => ModalTab = tab;

    public void ToggleReviewForm() => ShowReviewForm = !ShowReviewForm;

    public void SubmitReview()
    {
        if (ActiveModalRestaurant is null || string.IsNullOrWhiteSpace(NewReviewName)) return;
        ActiveModalRestaurant.Reviews.Insert(0, new Review(NewReviewName, "Just now", NewReviewRating, NewReviewComment));
        ActiveModalRestaurant.ReviewsCount++;
        NewReviewName = "";
        NewReviewComment = "";
        NewReviewRating = 5;
        ShowReviewForm = false;
    }

    public void TriggerBookingFromModal()
    {
        if (ActiveModalRestaurant is null) return;
        StartBooking(ActiveModalRestaurant);
    }

    public async Task StartBooking(Restaurant restaurant)
    {
        ActiveModalRestaurant = null;
        BookingRestaurant = restaurant;
        TimeSlot = restaurant.Slots[0];
        SelectedTable = null;
        BookingStep = 1;
        BookingConfirmed = false;
        PartySize = 2;
        DateShortcut = 0;
        BookingDate = DateTime.Today.ToString("yyyy-MM-dd");
        Occasion = "";
        Dietary.Clear();
        UiState.SetView("booking");
        await JavaScript.InvokeVoidAsync("scrollTo", new { top = 0, behavior = "smooth" });
    }

    public void GoToBookingStep(int step) => BookingStep = step;

    public void SelectDateShortcut(int offset)
    {
        DateShortcut = offset;
        BookingDate = DateTime.Today.AddDays(offset).ToString("yyyy-MM-dd");
    }

    public void SelectFloorTable(FloorTable table) => SelectedTable = table;

    public void ToggleOccasion(string occasion) => Occasion = Occasion == occasion ? "" : occasion;

    public void ToggleDietary(string item)
    {
        if (!Dietary.Add(item)) Dietary.Remove(item);
    }

    public void SubmitGuestDetails() => BookingStep = 4;

    public void ConfirmBooking()
    {
        if (BookingRestaurant is null) return;
        ConfirmationCode = $"#TBL-{Random.Shared.Next(1000, 9999)}-CPT";
        Bookings.Insert(0, new(ConfirmationCode, BookingRestaurant.Id, BookingRestaurant.Name, BookingDate, TimeSlot ?? "",
            PartySize, SelectedTable?.Name ?? "Host Selected", "Active"));
        BookingConfirmed = true;
    }

    public async Task SaveToAppleWallet() =>
        await JavaScript.InvokeVoidAsync("alert", "Added to Apple Wallet Passbook successfully!");

    public async Task ViewMyBookings()
    {
        BookingsTab = "active";
        await SwitchView("bookings");
    }

    public void SetBookingsTab(string tab) => BookingsTab = tab;

    public void CancelBooking(Booking booking) => Bookings.Remove(booking);

    public async Task ModifyBooking() =>
        await JavaScript.InvokeVoidAsync("alert", "Modification request sent to host.");

    public async Task RebookRestaurant(int restaurantId)
    {
        var restaurant = Restaurants.First(r => r.Id == restaurantId);
        await StartBooking(restaurant);
    }

    public async Task LookupBooking()
    {
        var query = LookupCode.Trim();
        if (string.IsNullOrEmpty(query)) return;

        var found = Bookings.FirstOrDefault(b => b.Ref.Equals(query, StringComparison.OrdinalIgnoreCase));
        var message = found is not null
            ? $"Reservation Found!\n\nVenue: {found.RestaurantName}\nDate: {found.Date} at {found.Time}\nTable: {found.Table}"
            : $"No active reservation found matching code: {query}";
        await JavaScript.InvokeVoidAsync("alert", message);
    }

    public async Task ToggleOpsTableStatus() =>
        await JavaScript.InvokeVoidAsync("alert", "Host Manager Action: Table state toggled between Seated / Cleaning / Available.");

    public async Task BlockTable() =>
        await JavaScript.InvokeVoidAsync("alert", "Block Table mode toggled for staff manager.");

    public void SeatGuest(Arrival arrival) => arrival.Seated = true;

    private static List<Restaurant> CreateRestaurants() => new()
    {
        new Restaurant
        {
            Id = 1, Name = "Ocean Grill & Raw Bar", Cuisine = "Seafood", Price = "$$$", Rating = 4.9, ReviewsCount = 420,
            Location = "V&A Waterfront, Cape Town", Address = "10 Breakwater Blvd, V&A Waterfront, Cape Town, 8001",
            CrossStreet = "Dock Road & Pierhead",
            Image = "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?auto=format&fit=crop&w=800&q=80",
            Badge = "Michelin Guide 2026",
            Slots = new[] { "18:00", "18:30", "19:00", "19:30", "20:00", "20:30" },
            Description = "Panoramic oceanfront views serving fresh line-caught oysters, prime dry-aged seafood, and handcrafted coastal cocktails.",
            DressCode = "Smart Casual", Parking = "Complimentary Valet at Victoria Mall", Corkage = "R150 per bottle",
            Menu = new()
            {
                new("Saldanha Bay Oysters (6)", "R210", "Fresh Raw Bar"),
                new("Seared Atlantic Tuna Steak", "R340", "Gluten-Free"),
                new("Cape Seafood Bouillabaisse", "R390", "Signature"),
                new("Charred Octopus & Caviar", "R280", "Chef Special")
            },
            Reviews = new()
            {
                new("Marcus Thorne", "2 days ago", 5, "Unbelievable sunset view over Table Mountain while enjoying the fresh oysters. Service by Sean was 10/10."),
                new("Elena Rostova", "1 week ago", 5, "The seared tuna was melted butter. Best dining spot at the V&A Waterfront!")
            }
        },
        new Restaurant
        {
            Id = 2, Name = "Le Petit Manoir", Cuisine = "French", Price = "$$$$", Rating = 4.95, ReviewsCount = 310,
            Location = "Franschhoek Valley", Address = "54 Huguenot Rd, Franschhoek, 7690",
            CrossStreet = "Main Road & Boucheron Street",
            Image = "https://images.unsplash.com/photo-1550966871-3ed3cdb5ed0c?auto=format&fit=crop&w=800&q=80",
            Badge = "World's 50 Best",
            Slots = new[] { "19:00", "19:30", "20:15" },
            Description = "Sophisticated modern French gastronomy using local artisanal produce paired with fine Cape Winelands vintages.",
            DressCode = "Formal / Elegant", Parking = "Private Estate Parking", Corkage = "R250 per bottle",
            Menu = new()
            {
                new("Pan-Seared Duck Breast", "R380", "French Classic"),
                new("Truffle & Forest Mushroom Risotto", "R320", "Vegetarian"),
                new("7-Course Chef Pairing Menu", "R1,250", "Tasting Menu")
            },
            Reviews = new() { new("David K.", "3 days ago", 5, "Exquisite wine pairing experience. Exceptional standard for Franschhoek.") }
        },
        new Restaurant
        {
            Id = 3, Name = "Kuro Omakase", Cuisine = "Japanese", Price = "$$$$", Rating = 4.88, ReviewsCount = 195,
            Location = "Camps Bay, Cape Town", Address = "88 Victoria Rd, Camps Bay, Cape Town, 8005",
            CrossStreet = "Victoria Rd & Argyle",
            Image = "https://images.unsplash.com/photo-1579027989536-b7b1f875659b?auto=format&fit=crop&w=800&q=80",
            Badge = "Chef's Table",
            Slots = new[] { "18:30", "20:30" },
            Description = "An intimate 14-seat counter Omakase experience featuring air-flown Tokyo market seafood and sake pairings.",
            DressCode = "Smart Casual", Parking = "Underground Bay Parking", Corkage = "No outside bottles",
            Menu = new()
            {
                new("18-Course Tokyo Omakase", "R1,450", "Authentic"),
                new("A5 Miyazaki Wagyu Nigiri", "R290", "Luxury Import")
            },
            Reviews = new() { new("Kenji Sato", "5 days ago", 5, "Authentic Tokyo sushi counter precision right in Camps Bay.") }
        },
        new Restaurant
        {
            Id = 4, Name = "La Trattoria del Sol", Cuisine = "Italian", Price = "$$", Rating = 4.76, ReviewsCount = 512,
            Location = "Constantia Upper", Address = "12 Constantia Main Rd, Constantia, 7806",
            CrossStreet = "Spaanschemat River Rd",
            Image = "https://images.unsplash.com/photo-1555396273-367ea4eb4db5?auto=format&fit=crop&w=800&q=80",
            Badge = "Top Terrace",
            Slots = new[] { "12:30", "13:00", "18:00", "19:30", "21:00" },
            Description = "Sunlit olive garden dining featuring hand-rolled woodfired pizzas, fresh pasta, and romantic courtyard tables.",
            DressCode = "Casual / Family", Parking = "On-site Lot", Corkage = "R100 per bottle",
            Menu = new()
            {
                new("Truffle Pappardelle", "R220", "Handmade"),
                new("Woodfired Burrata Pizza", "R195", "Artisanal")
            },
            Reviews = new() { new("Chloe Bennett", "Yesterday", 4, "Lovely outdoor terrace and delicious handmade pasta!") }
        },
        new Restaurant
        {
            Id = 5, Name = "The Botanical Table", Cuisine = "Farm-to-Table", Price = "$$$", Rating = 4.82, ReviewsCount = 240,
            Location = "Kirstenbosch Estate", Address = "Rhodes Dr, Newlands, Cape Town, 7735",
            CrossStreet = "Kirstenbosch Gate 2",
            Image = "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?auto=format&fit=crop&w=800&q=80",
            Badge = "Sustainable Winner",
            Slots = new[] { "12:00", "13:30", "18:30", "19:00" },
            Description = "Organic greenhouse cuisine surrounded by flora, featuring hyper-seasonal zero-waste tasting menus.",
            DressCode = "Smart Casual", Parking = "Estate Gate Parking", Corkage = "R120 per bottle",
            Menu = new()
            {
                new("Heirloom Tomato & Fynbos Tart", "R180", "Vegan"),
                new("Wild Herb Crusted Lamb Cutlets", "R330", "Local Farm")
            },
            Reviews = new() { new("Dr. Aris Thorne", "4 days ago", 5, "A serene garden atmosphere with incredible hyper-local flavors.") }
        }
    };

    public class Restaurant
    {
        public int Id { get; init; }
        public string Name { get; init; } = "";
        public string Cuisine { get; init; } = "";
        public string Price { get; init; } = "";
        public double Rating { get; init; }
        public int ReviewsCount { get; set; }
        public string Location { get; init; } = "";
        public string Address { get; init; } = "";
        public string CrossStreet { get; init; } = "";
        public string Image { get; init; } = "";
        public string Badge { get; init; } = "";
        public string[] Slots { get; init; } = Array.Empty<string>();
        public string Description { get; init; } = "";
        public string DressCode { get; init; } = "";
        public string Parking { get; init; } = "";
        public string Corkage { get; init; } = "";
        public List<Dish> Menu { get; init; } = new();
        public List<Review> Reviews { get; init; } = new();
    }

    public record Dish(string Name, string Price, string Tag);
    public record Review(string Name, string Date, int Rating, string Comment);
    public record Booking(string Ref, int RestaurantId, string RestaurantName, string Date, string Time, int Guests, string Table, string Status);
    public record FloorTable(string Name, string Description, int Capacity);

    public class Arrival(string time, string name, string detail, bool seated)
    {
        public string Time { get; } = time;
        public string Name { get; } = name;
        public string Detail { get; } = detail;
        public bool Seated { get; set; } = seated;
    }
}
