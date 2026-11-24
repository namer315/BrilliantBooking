using BrilliantBooking.Extensions;
using BrilliantBooking.Services;
using Data.Managers;
using Data.VO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BrilliantBooking.Components.Layout;

public partial class UserAccountMenu : ComponentBase, IAsyncDisposable
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private LoadingService LoadingService { get; set; } = default!;
    [Inject] private SessionService SessionService { get; set; } = default!;
    [Inject] private IMessageBoxService MessageBox { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    private ElementReference MenuElement;
    private DotNetObjectReference<UserAccountMenu>? DotNetReference;
    private bool IsMenuOpen { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && IsAuthenticated)
        {
            // To close the menu if i click out side the menu
            DotNetReference = DotNetObjectReference.Create(this);

            await JS.InvokeVoidAsync(
                "userAccountMenu.register",
                MenuElement,
                DotNetReference);
        }
    }

    [JSInvokable]
    public Task CloseFromOutside()
    {
        return InvokeAsync(() =>
        {
            if (IsMenuOpen)
            {
                IsMenuOpen = false;
                StateHasChanged();
            }
        });
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await JS.InvokeVoidAsync("userAccountMenu.unregister", MenuElement);
        }
        catch (JSDisconnectedException)
        {
        }

        DotNetReference?.Dispose();
    }

    private UserVO? CurrentUser => UserSessionManager.CurrentUser;

    private bool IsAuthenticated => UserSessionManager.IskeyExist && CurrentUser is not null;

    private UserVO.UserType CurrentUserType => CurrentUser switch
    {
        { Owner: not null } => UserVO.UserType.Owner,
        { Customer: not null } => UserVO.UserType.Client,
        not null => UserVO.UserType.Management,
        _ => UserVO.UserType.Client
    };

    private string RoleIcon => CurrentUserType switch
    {
        UserVO.UserType.Owner => "store",
        UserVO.UserType.Client => "user",
        UserVO.UserType.Management => "shield",
        _ => "user"
    };

    private string RoleLabel => CurrentUserType switch
    {
        UserVO.UserType.Owner => "Restaurant owner",
        UserVO.UserType.Client => "Client",
        UserVO.UserType.Management => "Management",
        _ => "Account"
    };

    private string RoleRoute => CurrentUserType switch
    {
        UserVO.UserType.Owner => "/App/Restaurant",
        UserVO.UserType.Client => "/Bookings",
        UserVO.UserType.Management => "/Explore",
        _ => "/Explore"
    };

    private string RoleRouteLabel => CurrentUserType switch
    {
        UserVO.UserType.Owner => "Restaurant dashboard",
        UserVO.UserType.Client => "My bookings",
        UserVO.UserType.Management => "Explore operations",
        _ => "Explore"
    };

    private void NavigateToLogin() => NavigationManager.NavigateTo("/login");

    private void ToggleMenu() => IsMenuOpen = !IsMenuOpen;

    private void CloseMenu() => IsMenuOpen = false;

    private void HandleKeyDown(KeyboardEventArgs args)
    {
        if (args.Key is "Escape" or "Esc")
        {
            CloseMenu();
        }
    }

    private async Task LogOut()
    {
        try
        {
            await SessionService.LogOut().ShowProcessing(LoadingService);
            IsMenuOpen = false;
            NavigationManager.NavigateTo("/" , forceLoad: true);
        }
        catch(Exception ex)
        {
            MessageBox.ShowException(ex);
        }
    }
}
