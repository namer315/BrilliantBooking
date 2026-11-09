using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BrilliantBooking.Components.Pages;

public partial class PageBase
{
    [Inject] public NavigationManager NavigationManager { get; set; }
    //[Inject] private ILogger<PageBase> Logger { get; set; }
    [Inject] public IJSRuntime JS { get; set; }


    public void ShowException(Exception ex , string title = "Error")
    {
        //MessageBox?.ShowException(ex , title);
    }
}