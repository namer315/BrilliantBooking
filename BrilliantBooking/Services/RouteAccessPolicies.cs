using Data.Managers;
using Data.VO;

namespace BrilliantBooking.Services;

public enum RouteVisibility
{
    Public,
    Private
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class RouteAccessAttribute : Attribute
{
    public UserVO.UserType[] AllowedUserTypes { get; }

    public RouteAccessAttribute(params UserVO.UserType[] allowedUserTypes)
    {
        AllowedUserTypes = allowedUserTypes;
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class NavigationRouteAttribute : Attribute
{
    public MenuVO.NavigationRoutes Route { get; }

    public NavigationRouteAttribute(MenuVO.NavigationRoutes route)
    {
        Route = route;
    }
}

public sealed record RouteAccessPolicy(
    RouteVisibility Visibility,
    IReadOnlySet<UserVO.UserType>? AllowedUserTypes = null)
{
    public bool Allows()
    {
        var isAuthenticated = UserSessionManager.IskeyExist
            && UserSessionManager.CurrentUser is not null;
        var userType = UserSessionManager.CurrentUserIfExists?.Type;

        if (Visibility != RouteVisibility.Private)
            return true;

        return isAuthenticated
            && (AllowedUserTypes is null
                || userType is not null && AllowedUserTypes.Contains(userType.Value));
    }
}

public static class RouteAccessPolicies
{
    public static RouteAccessPolicy Get(Type pageType)
    {
        var attribute = pageType.GetCustomAttributes(inherit: true)
            .OfType<RouteAccessAttribute>()
            .FirstOrDefault();

        // No user types means public. Supplied user types mean private access.
        return attribute is null || attribute.AllowedUserTypes.Length == 0
            ? new(RouteVisibility.Public)
            : new(RouteVisibility.Private, attribute.AllowedUserTypes.ToHashSet());
    }
}