using System;
using System.Globalization;
using System.Net;
using System.Text;
using Data.DTO.Common;
using Data.VO.Booking;

namespace Business.Common;

internal static class EmailBuilderBE
{
    public static /*(string Subject, string Recipient, string HtmlBody)*/EmailDTO CreateBookingEmail(BookingVO booking)
    {
        ArgumentNullException.ThrowIfNull(booking);

        //var recipient = booking.Customer?.Email?.Trim() ?? string.Empty;
        //var subject = BuildSubject(booking);
        //var htmlBody = BuildHtmlBody(booking);

        //return (subject, recipient, htmlBody);

        EmailDTO email = new EmailDTO();
        email.To = booking.Customer?.Email?.Trim() ?? string.Empty;
        email.Subject = BuildSubject(booking);
        email.Body = BuildHtmlBody(booking);

        return email;
    }

    private static string BuildSubject(BookingVO booking)
    {
        var restaurantName = GetValue(booking.Restaurant?.Name, "your restaurant");
        var reference = GetValue(booking.Reference, "reservation");

        var prefix = booking.Status switch
        {
            BookingVO.BookingStatus.Cancelled => "Reservation cancelled",
            BookingVO.BookingStatus.Past => "Reservation details",
            _ => "Reservation confirmed"
        };

        return $"{prefix} at {restaurantName} · {reference}";
    }

    private static (string Label, string Color, string Background, string Intro, string Closing) GetStatusContent(
        BookingVO.BookingStatus status)
    {
        return status switch
        {
            BookingVO.BookingStatus.Cancelled => (
                "Reservation cancelled",
                "#9b2c2c",
                "#fff5f5",
                "This reservation has been cancelled. The details are included below for your records.",
                "If you did not request this cancellation or would like to make a new reservation, please contact the restaurant directly."),
            BookingVO.BookingStatus.Past => (
                "Past reservation",
                "#5b6472",
                "#f4f6f8",
                "Thank you for dining with us. Here is a summary of your reservation for your records.",
                "We hope to welcome you again soon."),
            _ => (
                "Reservation confirmed",
                "#176b57",
                "#edf8f3",
                "Your table is reserved. We look forward to welcoming you and your guests.",
                "If your plans change, please contact the restaurant as soon as possible so they can assist you.")
        };
    }

    private static string BuildHtmlBody(BookingVO booking)
    {
        var status = GetStatusContent(booking.Status);
        var restaurant = booking.Restaurant;
        var customer = booking.Customer;
        var table = booking.BookingTable;
        var restaurantName = GetValue(restaurant?.Name, "Your restaurant");
        var customerName = GetValue(customer?.Name, "Guest");
        var reference = GetValue(booking.Reference, "Not provided");
        var date = booking.Date == default
            ? "Date to be confirmed"
            : booking.Date.ToString("dddd, MMMM d, yyyy", CultureInfo.InvariantCulture);
        var time = FormatTimeRange(booking.FromTime, booking.ToTime, booking.Time);

        var html = new StringBuilder(8000);
        html.Append("<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"UTF-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"></head>");
        html.Append("<body style=\"margin:0;padding:0;background-color:#f5f7f6;color:#24322e;font-family:Arial,Helvetica,sans-serif;\">");
        html.Append("<table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" width=\"100%\" style=\"width:100%;background-color:#f5f7f6;\"><tr><td align=\"center\" style=\"padding:32px 12px;\">");
        html.Append("<table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" width=\"600\" style=\"width:100%;max-width:600px;background-color:#ffffff;border:1px solid #e4ebe7;\">");

        html.Append("<tr><td style=\"padding:28px 32px;background-color:#173f35;\"><div style=\"font-size:12px;line-height:18px;letter-spacing:2px;color:#b9e2d3;text-transform:uppercase;\">Reservation details</div>");
        html.Append($"<div style=\"margin-top:8px;font-size:26px;line-height:32px;font-weight:bold;color:#ffffff;\">{Encode(restaurantName)}</div></td></tr>");

        html.Append($"<tr><td style=\"padding:28px 32px 12px;\"><div style=\"font-size:22px;line-height:30px;font-weight:bold;color:#173f35;\">Hello {Encode(customerName)},</div><div style=\"padding-top:12px;font-size:15px;line-height:24px;color:#52625d;\">{Encode(status.Intro)}</div></td></tr>");
        html.Append($"<tr><td style=\"padding:16px 32px;\"><table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" width=\"100%\"><tr><td style=\"padding:13px 16px;background-color:{status.Background};border-left:4px solid {status.Color};\"><div style=\"font-size:14px;line-height:20px;font-weight:bold;color:{status.Color};\">{Encode(status.Label)}</div><div style=\"padding-top:3px;font-size:13px;line-height:19px;color:#52625d;\">Reference: <strong>{Encode(reference)}</strong></div></td></tr></table></td></tr>");

        html.Append("<tr><td style=\"padding:12px 32px 0;\"><div style=\"font-size:11px;line-height:18px;letter-spacing:1.5px;color:#71817b;text-transform:uppercase;\">Your reservation</div>");
        html.Append("<table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" width=\"100%\" style=\"margin-top:8px;border-top:1px solid #e4ebe7;\">");
        AppendDetailRow(html, "Date", date);
        AppendDetailRow(html, "Time", time);
        AppendDetailRow(html, "Guests", FormatGuestCount(booking.GuestCount));
        AppendDetailRow(html, "Occasion", GetValue(booking.Occasion, "Not specified"));
        if (!string.IsNullOrWhiteSpace(booking.ConfirmationCode))
        {
            AppendDetailRow(html, "Confirmation code", booking.ConfirmationCode);
        }

        html.Append("</table></td></tr>");

        if (booking.Dietary.Count > 0 || !string.IsNullOrWhiteSpace(booking.Note))
        {
            html.Append("<tr><td style=\"padding:24px 32px 0;\"><div style=\"font-size:11px;line-height:18px;letter-spacing:1.5px;color:#71817b;text-transform:uppercase;\">Guest requests</div><table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" width=\"100%\" style=\"margin-top:8px;border-top:1px solid #e4ebe7;\">");
            if (booking.Dietary.Count > 0)
            {
                AppendDetailRow(html, "Dietary requirements", string.Join(", ", booking.Dietary));
            }

            if (!string.IsNullOrWhiteSpace(booking.Note))
            {
                AppendDetailRow(html, "Note", booking.Note);
            }

            html.Append("</table></td></tr>");
        }

        html.Append("<tr><td style=\"padding:24px 32px 0;\"><div style=\"font-size:11px;line-height:18px;letter-spacing:1.5px;color:#71817b;text-transform:uppercase;\">Restaurant information</div><table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" width=\"100%\" style=\"margin-top:8px;border-top:1px solid #e4ebe7;\">");
        AppendDetailRow(html, "Restaurant", restaurantName);
        if (!string.IsNullOrWhiteSpace(restaurant?.Address))
        {
            AppendDetailRow(html, "Address", restaurant.Address);
        }

        if (!string.IsNullOrWhiteSpace(restaurant?.Phone))
        {
            AppendDetailRow(html, "Phone", restaurant.Phone);
        }

        if (!string.IsNullOrWhiteSpace(restaurant?.Email))
        {
            AppendDetailRow(html, "Email", restaurant.Email);
        }

        if (table is not null)
        {
            var tableDetails = GetValue(table.Name, "Assigned on arrival");
            if (!string.IsNullOrWhiteSpace(table.Zone))
            {
                tableDetails += $" · {table.Zone}";
            }

            AppendDetailRow(html, "Table", tableDetails);
        }

        html.Append("</table></td></tr>");
        html.Append($"<tr><td style=\"padding:28px 32px 12px;\"><div style=\"font-size:15px;line-height:24px;color:#52625d;\">{Encode(status.Closing)}</div></td></tr>");
        html.Append("<tr><td style=\"padding:20px 32px 28px;background-color:#f8faf9;border-top:1px solid #e4ebe7;\"><div style=\"font-size:12px;line-height:19px;color:#71817b;\">Please keep this email for your records. If you contact the restaurant, have your reservation reference ready.</div></td></tr>");
        html.Append("</table></td></tr></table></body></html>");

        return html.ToString();
    }

    private static void AppendDetailRow(StringBuilder html, string label, string? value)
    {
        html.Append($"<tr><td width=\"38%\" style=\"padding:12px 8px 12px 0;border-bottom:1px solid #eef2f0;font-size:13px;line-height:20px;color:#71817b;vertical-align:top;\">{Encode(label)}</td><td style=\"padding:12px 0;border-bottom:1px solid #eef2f0;font-size:14px;line-height:20px;color:#24322e;vertical-align:top;\">{Encode(GetValue(value, "Not provided"))}</td></tr>");
    }

    private static string FormatTimeRange(TimeSpan fromTime, TimeSpan toTime, TimeSpan time)
    {
        if (fromTime != default || toTime != default)
        {
            var start = fromTime.ToString(@"hh\:mm", CultureInfo.InvariantCulture);
            var end = toTime.ToString(@"hh\:mm", CultureInfo.InvariantCulture);
            return fromTime == toTime ? start : $"{start} – {end}";
        }

        return time == default
            ? "Time to be confirmed"
            : time.ToString(@"hh\:mm", CultureInfo.InvariantCulture);
    }

    private static string FormatGuestCount(int guestCount) => guestCount == 1 ? "1 guest" : $"{guestCount} guests";

    private static string GetValue(string? value, string fallback) => string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();

    private static string Encode(string? value) => WebUtility.HtmlEncode(value ?? string.Empty);

}
