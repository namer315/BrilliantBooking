using BrilliantBooking.Extensions;
using Data.Search;
using Data.VO.Restaurant;
using FDM;

namespace BrilliantBooking.Components.Pages;

public partial class PageRestaurantList
{
    private RestaurantFDM _fdm = new();
    private RestaurantSH Search = new() { PageSize = 12 };
    private IList<RestaurantVO> RestaurantLsit = new List<RestaurantVO>();

    private long TotalRecords => Search.TotalRecords;
    private long TotalPages => Search.PageCount;
    private int CurrentPage => Search.PageNumber;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await dgSearch();
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    private async Task GoToPage(int pageNumber)
    {
        if (pageNumber < 1 || pageNumber > TotalPages || pageNumber == CurrentPage)
            return;

        Search.PageNumber = pageNumber;
        await dgSearch();
    }

    private Task PreviousPage() => GoToPage(CurrentPage - 1);

    private Task NextPage() => GoToPage(CurrentPage + 1);

    private async Task dgSearch()
    {
        RestaurantLsit = await _fdm.GetRestaurantLsitBy(Search).ShowProcessing(this);
    }

    private void OpenRestaurant(Guid id)
    {
        NavigationManager.NavigateTo($"/App/Restaurant/{id}");
    }
}