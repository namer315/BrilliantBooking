using Microsoft.AspNetCore.Components;

namespace BrilliantBooking.Components.Pages;

public partial class RestaurantCard : ComponentBase
{
    [Parameter, EditorRequired] public HomeBase.Restaurant Restaurant { get; set; } = null!;
    [Parameter] public bool IsFavorite { get; set; }
    [Parameter] public EventCallback<HomeBase.Restaurant> OnOpen { get; set; }
    [Parameter] public EventCallback<HomeBase.Restaurant> OnFavorite { get; set; }
    [Parameter] public EventCallback<HomeBase.Restaurant> OnReserve { get; set; }
}
