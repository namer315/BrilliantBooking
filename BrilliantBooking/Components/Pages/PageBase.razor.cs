using BrilliantBooking.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BrilliantBooking.Components.Pages;

public partial class PageBase
{
    [Inject] public NavigationManager NavigationManager { get; set; }
    //[Inject] private ILogger<PageBase> Logger { get; set; }
    [Inject] public IJSRuntime JS { get; set; }

    protected SessionService _sessionService;
    [Inject]
    protected SessionService SessionService
    {
        get
        {
            return _sessionService;
        }
        set
        {
            _sessionService = value;
            value.SendOnlineUserKeyToBackEnd();
        }
    }

    [Inject] private IMessageBoxService MessageBoxService { get; set; } = default!;
    [Inject] public LoadingService LoadingService { get; set; } = default!;



    #region Parameter
    public string PageMode { get; set; } = null;
    // The raw string from the URL route (e.g., "New", "new", "d3b07384-...")
    [Parameter] public string? Id { get; set; }
    //public bool IsNew => Id is not null && string.Equals(Id , RouteManager.New , StringComparison.OrdinalIgnoreCase);

    #endregion

    #region life
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        //if (firstRender)
        {
            await JS.InvokeVoidAsync("refreshReserveIcons");
            StateHasChanged();
        }
    }
    #endregion

    /// <summary>
    /// Parses the route ID. Returns null if "New", a Guid if valid, or throws if invalid.
    /// </summary>
    protected Guid? GetEntityId()
    {
        // 1. Handle "New" (Case-insensitive)
        if (string.Equals(Id , /*RouteManager.New*/ "New" , StringComparison.OrdinalIgnoreCase))
        {
            return null; // Null implies "Create Mode"
        }

        // 2. Handle Guid (Edit Mode)
        if (Guid.TryParse(Id , out var parsedGuid) && parsedGuid != Guid.Empty)
        {
            return parsedGuid;
        }

        // 3. Invalid Route -> Throw
        //NavigateToErroPage($"Invalid route parameter '{Id}'. Must be a GUID or 'new'.");
        throw new ArgumentException($"The route parameter '{Id}' is invalid. It must be a GUID or 'New'.");
    }

    #region IMessageBoxService
    public void ShowMessage(string text , string title = "Confirmation")
    {
        MessageBoxService?.ShowMessage(text , title);
    }
    public void ShowException(Exception ex , string title = "Error")
    {
        MessageBoxService.ShowException(ex , title);
    }
    public void ShowWarning(string text , string title = "Warning")
    {
        MessageBoxService?.ShowWarning(text , title);
    }

    #endregion
}