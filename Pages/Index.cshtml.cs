using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ERPBooking.Pages;

public sealed class IndexModel : PageModel
{
    public string ApplicationName { get; private set; } = string.Empty;
    public string PageTitle { get; private set; } = string.Empty;
    public string WorkspaceLabel { get; private set; } = string.Empty;
    public string IntroText { get; private set; } = string.Empty;

    public void OnGet()
    {
        ApplicationName = "ERPBooking";
        PageTitle = "ERPBooking";
        WorkspaceLabel = "BOOKING WORKSPACE";
        IntroText = "Your web app is ready to grow. This Razor Pages interface is served by ASP.NET Core, with a C# Minimal API behind it.";
    }
}
