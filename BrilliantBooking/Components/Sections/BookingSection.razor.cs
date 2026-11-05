using BrilliantBooking.Components.Pages;
using Microsoft.AspNetCore.Components;

namespace BrilliantBooking.Components.Sections;

public partial class BookingSection : ComponentBase
{
    [Parameter, EditorRequired] public HomeBase Page { get; set; } = null!;
}
