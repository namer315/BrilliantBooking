using BrilliantBooking.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BrilliantBooking.Components.Pages;

public partial class PageBase
{
    [Inject] public NavigationManager NavigationManager { get; set; }
    //[Inject] private ILogger<PageBase> Logger { get; set; }
    [Inject] public IJSRuntime JS { get; set; }

    [Inject] private IMessageBoxService MessageBoxService { get; set; } = default!;

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