using Data.Services;
using Data.VO.Restaurant;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

namespace BrilliantBooking.Extensions;

public static class NavigationExtensions
{
    public static void NavigateToBooking(this NavigationManager NavigationManager , Guid? id = null , RestaurantVO restaurant = null)
    {
        IDictionary<string , string?> queryString = new Dictionary<string , string>();

        if (restaurant is not null)
            queryString["qR"] = restaurant.Id.ToString();
        string uri;
        if (id is null)
        {
            uri = RouteUrlService.UrlNew(Data.VO.MenuVO.NavigationRoutes.Booking, queryString);
        }
        else
            uri = RouteUrlService.Url(Data.VO.MenuVO.NavigationRoutes.Booking , queryString);

        NavigationManager.NavigateTo(uri , true);
    }
}
