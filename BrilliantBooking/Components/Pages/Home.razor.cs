using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BrilliantBooking.Components.Pages;

public partial class HomeBase : ComponentBase
{
    [Inject] protected IJSRuntime JavaScript { get; set; } = null!;
    protected IReadOnlyList<(string Value, string Icon, string Label)> NavItems { get; } = new[]
    {
        ("explore", "compass", "Explore"), ("bookings", "calendar-check", "My Bookings"), ("ops", "layout-dashboard", "Ops")
    };
    protected string View { get; set; } = "explore";
    protected bool IsDark { get; set; }
    protected string Location { get; set; } = "Cape Town, SA";
    protected bool LocationModal { get; set; }
    protected bool MobileMenuOpen { get; set; }
    protected bool FavoritesOnly { get; set; }
    protected string ModalTab { get; set; } = "overview";
    protected string Search { get; set; } = "";
    protected string Cuisine { get; set; } = "all";
    protected string Price { get; set; } = "all";
    protected string Category { get; set; } = "all";
    protected HashSet<int> Favorites { get; } = new();
    protected Restaurant? SelectedRestaurant { get; set; }
    protected Restaurant? BookingRestaurant { get; set; }
    protected int BookingStep { get; set; }
    protected int PartySize { get; set; } = 2;
    protected DateTime BookingDate { get; set; } = DateTime.Today;
    protected int HeroPartySize { get; set; } = 2;
    protected DateTime HeroDate { get; set; } = DateTime.Today;
    protected string? TimeSlot { get; set; }
    protected string GuestName { get; set; } = "";
    protected string GuestEmail { get; set; } = "";
    protected string GuestPhone { get; set; } = "";
    protected string GuestNotes { get; set; } = "";
    protected string Occasion { get; set; } = "";
    protected HashSet<string> DietaryRequirements { get; } = new();
    protected TableNode? BookingTable { get; set; }
    protected TableNode? SelectedTable { get; set; }
    protected string ConfirmationCode { get; set; } = "#TBL-4821-CPT";
    protected string BookingFilter { get; set; } = "active";

    protected IReadOnlyList<(string Value, string Label)> Categories { get; } = new[]
    {
        ("all", "All Venues"), ("Seafood", "Waterfront & Seafood"), ("French", "Michelin & Fine Dining"),
        ("Japanese", "Japanese Omakase"), ("Italian", "Terrace Italian"), ("Farm-to-Table", "Farm-to-Table")
    };
    protected IReadOnlyList<string> Locations { get; } = new[] { "Cape Town, SA", "Franschhoek, Winelands", "Constantia Valley" };
    protected IReadOnlyList<string> Occasions { get; } = new[] { "Birthday", "Anniversary", "Business Meal", "Date Night" };
    protected IReadOnlyList<string> DietaryOptions { get; } = new[] { "Vegan / Plant", "Gluten-Free", "Halal", "Nut Allergy", "Dairy Free" };
    protected List<Restaurant> Restaurants { get; } = CreateRestaurants();
    protected List<Booking> Bookings { get; } = new()
    {
        new("#TBL-1182-CPT", "Ocean Grill & Raw Bar", "2026-11-02", "19:30", 4, "Table 04", "Active"),
        new("#TBL-9031-CPT", "Le Petit Manoir", "2026-08-14", "20:15", 2, "Chef Counter", "Past")
    };
    protected List<TableNode> Tables { get; } = new()
    {
        new("T-01", 120, 130, 4, true, false), new("T-02", 270, 130, 2, true, true),
        new("T-04", 420, 130, 4, false, false), new("T-12", 180, 280, 2, false, false),
        new("P-01", 620, 140, 2, true, false), new("P-02", 690, 290, 4, true, false)
    };
    protected List<Arrival> Arrivals { get; } = new()
    {
        new("19:30", "Vance Party", 4, "Table 04 • Gluten-Free", true),
        new("19:45", "Dr. A. Smith", 2, "Table 12 • VIP", false),
        new("20:15", "Claire Dupont", 6, "Patio Booth P1 • Birthday", false)
    };

    protected IEnumerable<Restaurant> FilteredRestaurants => Restaurants.Where(r =>
        (Cuisine == "all" || r.Cuisine == Cuisine) && (Price == "all" || r.Price == Price) &&
        (Category == "all" || r.Cuisine == Category) &&
        (!FavoritesOnly || Favorites.Contains(r.Id)) &&
        (string.IsNullOrWhiteSpace(Search) || $"{r.Name} {r.Cuisine} {r.Location}".Contains(Search, StringComparison.OrdinalIgnoreCase)));
    protected IEnumerable<Booking> VisibleBookings => Bookings.Where(b => b.Status.Equals(BookingFilter == "active" ? "Active" : "Past", StringComparison.OrdinalIgnoreCase));

    protected void SwitchView(string view) => View = view;
    protected void GoExplore() => SwitchView("explore");
    protected void SearchClick() => View = "explore";
    protected void ToggleMobileMenu() => MobileMenuOpen = !MobileMenuOpen;
    protected void MobileView(string view) { View = view; MobileMenuOpen = false; }
    protected void MobileExplore() => MobileView("explore");
    protected void MobileBookings() => MobileView("bookings");
    protected void MobileOps() => MobileView("ops");
    protected void ToggleFavoritesOnly() { FavoritesOnly = !FavoritesOnly; View = "explore"; }
    protected void ShowActiveBookings() => BookingFilter = "active";
    protected void ShowPastBookings() => BookingFilter = "past";
    protected void CancelReservation(Booking booking) => Bookings.Remove(booking);
    protected void ToggleTheme() => IsDark = !IsDark;
    protected void SetCategory(string value) => Category = value;
    protected void ResetFilters() { Search = ""; Cuisine = "all"; Price = "all"; Category = "all"; FavoritesOnly = false; }
    protected void SetLocation(string value) { Location = value; LocationModal = false; }
    protected void OpenRestaurant(Restaurant restaurant) { SelectedRestaurant = restaurant; ModalTab = "overview"; }
    protected void CloseRestaurant() => SelectedRestaurant = null;
    protected void ToggleFavorite(Restaurant restaurant) { if (!Favorites.Add(restaurant.Id)) Favorites.Remove(restaurant.Id); }
    protected void SetModalTab(string tab) => ModalTab = tab;
    protected void SelectOverviewTab() => SetModalTab("overview");
    protected void SelectMapTab() => SetModalTab("map");
    protected void SelectReviewsTab() => SetModalTab("reviews");
    protected void ToggleOccasion(string occasion) => Occasion = Occasion == occasion ? "" : occasion;
    protected void ToggleDietary(string item) { if (!DietaryRequirements.Add(item)) DietaryRequirements.Remove(item); }
    protected void StartBooking(Restaurant restaurant) { SelectedRestaurant = null; BookingRestaurant = restaurant; BookingStep = 1; TimeSlot = null; BookingTable = null; Occasion = ""; DietaryRequirements.Clear(); }
    protected void CancelBooking() { BookingRestaurant = null; BookingStep = 0; }
    protected bool CanAdvanceBooking => BookingStep switch
    {
        1 => !string.IsNullOrWhiteSpace(TimeSlot),
        2 => BookingTable is not null,
        3 => !string.IsNullOrWhiteSpace(GuestName) && !string.IsNullOrWhiteSpace(GuestEmail) && !string.IsNullOrWhiteSpace(GuestPhone),
        _ => false
    };

    protected void NextBookingStep()
    {
        if (!CanAdvanceBooking) return;
        if (BookingStep == 3)
        {
            ConfirmationCode = $"#TBL-{Random.Shared.Next(1000, 9999)}-CPT";
            Bookings.Insert(0, new(ConfirmationCode, BookingRestaurant?.Name ?? "Restaurant", BookingDate.ToString("yyyy-MM-dd"), TimeSlot ?? "", PartySize, BookingTable?.Name ?? "Host assigned", "Active"));
        }
        BookingStep = Math.Min(4, BookingStep + 1);
    }
    protected void PreviousBookingStep() { if (BookingStep > 1 && BookingStep < 4) BookingStep--; }
    protected void SelectTable(TableNode table) { if (!table.Occupied) SelectedTable = table; }
    protected void BlockTable() { }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JavaScript.InvokeVoidAsync("refreshReserveIcons");
    }

    private static List<Restaurant> CreateRestaurants() => new()
    {
        new(1, "Ocean Grill & Raw Bar", "Seafood", "$$$", 4.9, 420, "V&A Waterfront, Cape Town", "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?auto=format&fit=crop&w=800&q=80", "Michelin Guide 2026", "Panoramic oceanfront views serving fresh line-caught oysters, prime dry-aged seafood, and handcrafted coastal cocktails.", "Smart Casual", "Complimentary Valet at Victoria Mall", new[] { "18:00", "18:30", "19:00", "19:30", "20:00", "20:30" }, new Dish("Saldanha Bay Oysters (6)", "R210", "Fresh Raw Bar"), new Dish("Seared Atlantic Tuna Steak", "R340", "Gluten-Free"), new Dish("Cape Seafood Bouillabaisse", "R390", "Signature"), new Dish("Charred Octopus & Caviar", "R280", "Chef Special")) { Address = "10 Breakwater Blvd, V&A Waterfront, Cape Town, 8001", CrossStreet = "Dock Road & Pierhead", Corkage = "R150 per bottle", Reviews = new[] { new Review("Marcus Thorne", "2 days ago", 5, "Unbelievable sunset view over Table Mountain while enjoying the fresh oysters. Service by Sean was 10/10."), new Review("Elena Rostova", "1 week ago", 5, "The seared tuna was melted butter. Best dining spot at the V&A Waterfront!") } },
        new(2, "Le Petit Manoir", "French", "$$$$", 4.95, 310, "Franschhoek Valley", "https://images.unsplash.com/photo-1550966871-3ed3cdb5ed0c?auto=format&fit=crop&w=800&q=80", "World's 50 Best", "Sophisticated modern French gastronomy using local artisanal produce paired with fine Cape Winelands vintages.", "Formal / Elegant", "Private Estate Parking", new[] { "19:00", "19:30", "20:15" }, new Dish("Pan-Seared Duck Breast", "R380", "French Classic"), new Dish("Truffle & Forest Mushroom Risotto", "R320", "Vegetarian"), new Dish("7-Course Chef Pairing Menu", "R1,250", "Tasting Menu")) { Address = "54 Huguenot Rd, Franschhoek, 7690", CrossStreet = "Main Road & Boucheron Street", Corkage = "R250 per bottle", Reviews = new[] { new Review("David K.", "3 days ago", 5, "Exquisite wine pairing experience. Exceptional standard for Franschhoek.") } },
        new(3, "Kuro Omakase", "Japanese", "$$$$", 4.88, 195, "Camps Bay, Cape Town", "https://images.unsplash.com/photo-1579027989536-b7b1f875659b?auto=format&fit=crop&w=800&q=80", "Chef's Table", "An intimate 14-seat counter Omakase experience featuring air-flown Tokyo market seafood and sake pairings.", "Smart Casual", "Underground Bay Parking", new[] { "18:30", "20:30" }, new Dish("18-Course Tokyo Omakase", "R1,450", "Authentic"), new Dish("A5 Miyazaki Wagyu Nigiri", "R290", "Luxury Import")) { Address = "88 Victoria Rd, Camps Bay, Cape Town, 8005", CrossStreet = "Victoria Rd & Argyle", Corkage = "No outside bottles", Reviews = new[] { new Review("Kenji Sato", "5 days ago", 5, "Authentic Tokyo sushi counter precision right in Camps Bay.") } },
        new(4, "La Trattoria del Sol", "Italian", "$$", 4.76, 512, "Constantia Upper", "https://images.unsplash.com/photo-1555396273-367ea4eb4db5?auto=format&fit=crop&w=800&q=80", "Top Terrace", "Sunlit olive garden dining featuring hand-rolled woodfired pizzas, fresh pasta, and romantic courtyard tables.", "Casual / Family", "On-site Lot", new[] { "12:30", "13:00", "18:00", "19:30", "21:00" }, new Dish("Truffle Pappardelle", "R220", "Handmade"), new Dish("Woodfired Burrata Pizza", "R195", "Artisanal")) { Address = "12 Constantia Main Rd, Constantia, 7806", CrossStreet = "Spaanschemat River Rd", Corkage = "R100 per bottle", Reviews = new[] { new Review("Chloe Bennett", "Yesterday", 4, "Lovely outdoor terrace and delicious handmade pasta!") } },
        new(5, "The Botanical Table", "Farm-to-Table", "$$$", 4.82, 240, "Kirstenbosch Estate", "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?auto=format&fit=crop&w=800&q=80", "Editor's Pick", "Seasonal tasting menus celebrating the growers, winemakers and wild landscapes of the Cape.", "Elegant Casual", "Complimentary Parking", new[] { "18:30", "19:30" }, new Dish("Seasonal Tasting", "R690", "Chef's Menu"), new Dish("Heritage Lamb", "R360", "Local Produce")) { Address = "Rhodes Dr, Newlands, Cape Town", CrossStreet = "Kirstenbosch Gate", Corkage = "R150 per bottle", Reviews = new[] { new Review("Amara N.", "1 week ago", 5, "A beautiful seasonal menu in an unforgettable setting.") } }
    };

    public record Restaurant(int Id, string Name, string Cuisine, string Price, double Rating, int ReviewsCount, string Location, string Image, string Badge, string Description, string DressCode, string Parking, string[] Slots, params Dish[] Menu)
    {
        public string Address { get; init; } = Location;
        public string CrossStreet { get; init; } = "Main Boulevard";
        public string Corkage { get; init; } = "Standard fee";
        public string CancellationPolicy { get; init; } = "2 Hours Free";
        public string Source { get; init; } = "Internal";
        public IReadOnlyList<Review> Reviews { get; init; } = Array.Empty<Review>();
    }
    public record Dish(string Name, string Price, string Tag);
    public record Review(string Name, string Date, int Rating, string Comment)
    {
        public string Source { get; init; } = "Guest";
    }
    public record Booking(string Reference, string RestaurantName, string Date, string Time, int Guests, string Table, string Status);
    public record TableNode(string Name, int X, int Y, int Capacity, bool Round, bool Occupied);
    protected class Arrival(string time, string name, int guests, string table, bool seated) { public string Time { get; } = time; public string Name { get; } = name; public int Guests { get; } = guests; public string Table { get; } = table; public bool Seated { get; set; } = seated; }
}
