using BrilliantBooking.Extensions;
using Data.Managers;
using Data.Search;
using Data.VO;
using BrilliantBooking.Services;
using Data.VO.Restaurant;
using FDM;
using static Data.VO.MenuVO;

namespace BrilliantBooking.Components.Pages;

[NavigationRoute(NavigationRoutes.Resturant)]
[RouteAccess(UserVO.UserType.Owner, UserVO.UserType.Management)]
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
            Search.Owner = UserSessionManager.CurrentUser.Owner;
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

    private string GetImageSrc(RestaurantVO restaurant)
    {
        var photo = restaurant.PhotoList.FirstOrDefault();
        return photo?.Photo is { Length: > 0 } bytes
            ? bytes.ToImage()
            : "";
    }
}