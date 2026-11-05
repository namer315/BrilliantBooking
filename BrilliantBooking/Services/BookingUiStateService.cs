namespace BrilliantBooking.Services;

public class BookingUiStateService
{
    public string View { get; private set; } = "explore";
    public bool IsDark { get; private set; }
    public bool MobileMenuOpen { get; private set; }
    public bool LocationModalOpen { get; private set; }
    public bool FavoritesOnly { get; private set; }
    public string Location { get; private set; } = "Cape Town, SA";
    public HashSet<int> Favorites { get; } = new();

    public event Action? Changed;

    public void SwitchView(string v) => SetView(v);
    public void SetView(string view)
    {
        View = view;
        MobileMenuOpen = false;
        Notify();
    }

    public void ToggleTheme()
    {
        IsDark = !IsDark;
        Notify();
    }

    public void ToggleMobileMenu()
    {
        MobileMenuOpen = !MobileMenuOpen;
        Notify();
    }

    public void ToggleLocationModal()
    {
        LocationModalOpen = !LocationModalOpen;
        Notify();
    }

    public void SetLocation(string location)
    {
        Location = location;
        LocationModalOpen = false;
        Notify();
    }

    public void ToggleFavoritesOnlyFilter()
    {
        FavoritesOnly = !FavoritesOnly;
        Notify();
    }

    public void ToggleFavorite(int restaurantId)
    {
        if (!Favorites.Add(restaurantId))
        {
            Favorites.Remove(restaurantId);
        }
        Notify();
    }

    private void Notify() => Changed?.Invoke();
}
