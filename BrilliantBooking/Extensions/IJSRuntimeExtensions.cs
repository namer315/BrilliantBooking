using Microsoft.JSInterop;

namespace BrilliantBooking.Extensions;

public static class IJSRuntimeExtensions
{
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
