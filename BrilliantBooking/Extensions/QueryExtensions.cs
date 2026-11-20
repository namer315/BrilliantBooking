using Microsoft.AspNetCore.Components;
using System.Data.SqlTypes;

namespace BrilliantBooking.Extensions;

public static class QueryExtensions
{
    public static void ValidateQueryGuidParameter(this string? input , NavigationManager navigationManager , string queryParameter , Action<Guid> onValidGuid)
    {
        if (input is null)
            return;

        if (Guid.TryParse(input , out var parsedGuid) && parsedGuid != Guid.Empty)
        {
            onValidGuid(parsedGuid);
            // ⚠️ Add a return here if you don’t want to navigate to the error page on success
            return;
        }


        navigationManager.NavigateToErroPage($"Invalid {queryParameter}: {input}");
    }
    public static void ValidateQueryDecimalParameter(this string? input , NavigationManager navigationManager , string queryParameter , Action<decimal> onValidDecimal)
    {
        if (input is null)
            return;

        if (decimal.TryParse(input , out var parsedDecimal))
        {
            onValidDecimal(parsedDecimal);
            // ⚠️ Add a return here if you don’t want to navigate to the error page on success
            return;
        }


        navigationManager.NavigateToErroPage($"Invalid {queryParameter}: {input}");
    }
    public static void ValidateQueryDateTimeParameter(this string? input , NavigationManager navigationManager , string queryParameter , Action<DateTime> onValidDateTime)
    {
        if (input is null)
            return;

        if (DateTime.TryParse(input , out var parsedDate) && parsedDate > SqlDateTime.MinValue.Value)
        {
            onValidDateTime(parsedDate);
            // ⚠️ Add a return here if you don’t want to navigate to the error page on success
            return;
        }

        navigationManager.NavigateToErroPage($"Invalid {queryParameter}: {input}");
    }
}
