namespace ERPBooking.Booking;

public sealed record Reservation(
    string Reference,
    string RestaurantSlug,
    string GuestName,
    string Email,
    string Phone,
    DateOnly Date,
    TimeOnly Time,
    int Guests,
    DateTime CreatedAtUtc);

public enum ReservationResult
{
    Confirmed,
    Unavailable
}
