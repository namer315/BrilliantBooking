using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using static Data.VO.MenuVO;

namespace Data.Services;

public sealed class RouteUrlService
{
    public static string New => "New";
    public static string Url(NavigationRoutes route)
        => GetBaseSegment(route);
    public static string Url(NavigationRoutes route , IDictionary<string , string?> queryString)
        => QueryHelpers.AddQueryString(Url(route) , queryString);

    public static string Url(NavigationRoutes route , Guid? id = null)
    {
        if (id is null)
            return GetBaseSegment(route);

        if (id == Guid.Empty)
            return $"{GetBaseSegment(route)}/{New}";

        return $"{GetBaseSegment(route)}/{id.Value:D}";
    }
    public static string Url(NavigationRoutes route , Guid? id = null, IDictionary<string , string?> queryString = null)
        => queryString is not { Count: > 0 } ? Url(route , id) : QueryHelpers.AddQueryString(Url(route , id) , queryString);


    public static string UrlNew(NavigationRoutes route)
        => $"{GetBaseSegment(route)}/{New}";
    public static string UrlNew(NavigationRoutes route, IDictionary<string , string?> queryString)
         => QueryHelpers.AddQueryString(UrlNew(route) , queryString);

    public static string GetDetailsUrl(NavigationRoutes route , Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Use GetNewUrl or pass Guid.Empty for a new item." , nameof(id));

        return $"{GetBaseSegment(route)}/{id:D}";
    }
    public static string GetDetailsUrl(NavigationRoutes route , Guid id , IDictionary<string , string?> queryString)
        => QueryHelpers.AddQueryString(Url(route) , queryString);

    private static string GetBaseSegment(NavigationRoutes route)
    {
        //return $"/{route.ToString()}";
        return route switch
        {
            //NavigationRoutes.LogIn => "/login",
            //NavigationRoutes.LogOut => "/logout",
            //NavigationRoutes.Booking => "/booking",
            //NavigationRoutes.Customers => "/customers",
            NavigationRoutes.Resturant => "App/Resturant",
            NavigationRoutes.Dashboard => "/Owner/Dashboard",
            _ => $"/{route.ToString()}"
            //_ => throw new ArgumentOutOfRangeException(nameof(route) , route , "Unsupported route enum value.")
        };
    }
}