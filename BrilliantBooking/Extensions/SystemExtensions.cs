using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace BrilliantBooking.Extensions;

public static class SystemExtensions
{
    public static void NavigateToErroPage(this HttpContext context , string errorMsg)
    {
        var uri = QueryHelpers.AddQueryString("/Error" , new Dictionary<string , string>
        {
            ["ErrorMessage"] = errorMsg ,
            ["SourceUrl"] = context.Request.Path
        });
        context.Response.Redirect(uri , permanent: true);
    }

    public static void NavigateToErroPage(this NavigationManager NavigationManager , string errorMessage)
    {
        var uri = QueryHelpers.AddQueryString("/Error" , new Dictionary<string , string>
        {
            ["ErrorMessage"] = errorMessage ,
            ["SourceUrl"] = NavigationManager.Uri
        });
        NavigationManager.NavigateTo(uri);
    }
}
