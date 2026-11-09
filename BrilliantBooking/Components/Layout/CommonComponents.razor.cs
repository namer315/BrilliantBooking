using BrilliantBooking.Services;
using Microsoft.AspNetCore.Components;

namespace BrilliantBooking.Components.Layout;

public partial class CommonComponents : IDisposable
{
    [Inject] private IMessageBoxService MessageBoxService { get; set; } = default!;

    protected override void OnInitialized()
    {
        MessageBoxService.Changed += OnMessageBoxChanged;
    }

    private void OnMessageBoxChanged() => InvokeAsync(StateHasChanged);

    public void Dispose()
    {
        MessageBoxService.Changed -= OnMessageBoxChanged;
    }
}
