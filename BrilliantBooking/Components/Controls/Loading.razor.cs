using BrilliantBooking.Services;
using Microsoft.AspNetCore.Components;

namespace BrilliantBooking.Components.Controls;

public partial class Loading : IDisposable
{
    [Inject] private LoadingService Loader { get; set; } = default!;

    [Parameter] public string MainText { get; set; } = "Processing";

    [Parameter] public string SubText { get; set; } = "Please wait a moment while we set things up";

    protected override void OnInitialized()
    {
        Loader.Changed += OnLoadingChanged;
    }

    private void OnLoadingChanged() => InvokeAsync(StateHasChanged);

    public void Dispose()
    {
        Loader.Changed -= OnLoadingChanged;
    }
}
