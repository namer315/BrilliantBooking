using BrilliantBooking.Services;
using Data.Search;
using Data.VO.Restaurant;
using FDM;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using static BrilliantBooking.Components.Pages.HomeBase;

namespace BrilliantBooking.Components.Pages;

public partial class PageExplore
{
    private RestaurantFDM _fdm = new();
    private RestaurantSH Search = new();
    private IList<RestaurantVO> RestaurantLsit = new List<RestaurantVO>();
    public IList<string> CuisineList => RestaurantLsit.Select(x => x.Cuisine).ToList();

    public IReadOnlyList<(string Value , string Label)> Prices { get; } = new[]
    {
        ("all", "All Prices"), ("$$", "$$ (Moderate)"), ("$$$", "$$$ (Fine)"), ("$$$$", "$$$$ (Luxury)")
    };

    public IReadOnlyList<(string Value , string Label)> Categories { get; } = new[]
    {
        ("all", "All Venues"), ("Seafood", "Waterfront & Seafood"), ("French", "Michelin & Fine Dining"),
        ("Japanese", "Japanese Omakase"), ("Italian", "Terrace Italian")
    };


    [Inject] public BookingUiStateService UiState { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            dgSearch();
        }
        catch(Exception ex)
        {

        }
    }

    private async Task dgSearch()
    {
        RestaurantLsit = await _fdm.GetRestaurantLsitBy(Search);
    }

    public async void Reset()
    {
        Search.Text = string.Empty;
        await dgSearch();
    }

    public void ToggleFavorite(RestaurantVO restaurant)
    {

    }


    private void OpenRestaurant(RestaurantVO item)
    {

    }


    private void StartBooking(RestaurantVO item)
    {
    }
}