using Microsoft.JSInterop;

namespace BrilliantBooking.Extensions;

public static class IJSRuntimeExtensions
{
    public static async ValueTask RefreshReserveIconsAsync(
        this IJSRuntime jsRuntime,
        CancellationToken disposalToken,
        ILogger logger)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("refreshReserveIcons", disposalToken);
        }
        catch (OperationCanceledException ex) when (disposalToken.IsCancellationRequested)
        {
            logger.LogDebug(ex, "Icon refresh canceled because the component was disposed.");
        }
        catch (JSDisconnectedException ex)
        {
            logger.LogDebug(ex, "Icon refresh stopped because the browser circuit disconnected.");
        }
    }

    public static ValueTask SetCookieAsync(
        this IJSRuntime jsRuntime,
        string name,
        string value,
        int days = 30)
    {
        return jsRuntime.InvokeVoidAsync("setCookie", name, value, days);
    }

    public static ValueTask RemoveCookieAsync(this IJSRuntime jsRuntime, string name)
    {
        return jsRuntime.InvokeVoidAsync("removeCookie", name);
    }
}
