using Data.VO.Customer;
using Data.VO.Operations;
using Data.VO.Restaurant;
using NHibernate.Type;

namespace Data.VO.Booking;

/// <summary>
/// Table reservation made by a customer.
/// Source: myBookings[] and bookingState in index4.html.
/// </summary>
public class BookingVO : EntityBaseWithCode
{
    //public int PartySize { get; set; } = 2
    public virtual string Reference { get; set; } = string.Empty;

    public virtual DateOnly Date { get; set; }
    public virtual TimeSpan Time { get; set; }
    public virtual TimeSpan FromTime { get; set; }
    public virtual TimeSpan ToTime { get; set; }
    public virtual int GuestCount { get; set; }
    //public virtual string TableName { get; set; } = string.Empty;
    public virtual BookingStatus Status { get; set; } = BookingStatus.Active;
    public virtual string Occasion { get; set; } = string.Empty;
    //public virtual string Dietary { get; set; } = string.Empty;

    public virtual HashSet<string> Dietary { get; } = new();
    /// <summary>
    /// read-only
    /// </summary>
    public virtual int Step { get; set; } = 1;
    public virtual string Note { get; set; }
    public virtual bool BookingConfirmed { get; set; } = false;
    public virtual string ConfirmationCode { get; set; } = string.Empty;

    public virtual RestaurantVO Restaurant { get; set; } = null!;
    public virtual CustomerVO Customer { get; set; } = null!;
    public virtual TableVO BookingTable { get; set; }

    public enum BookingStatus
    {
        //Booked,
        Active,
        CheckedIn,        // Customer has arrived or service has started
        Completed,
        Past,
        Cancelled
    }
    /*public enum BookingStatus
    {
        Unknown = 0,

        Pending = 1,          // Created, waiting for customer confirmation/payment
        PendingPayment = 2,   // Awaiting payment
        Confirmed = 3,        // Payment received / booking accepted
        CheckedIn = 4,        // Customer has arrived or service has started
        InProgress = 5,       // Booking/service is currently active
        Completed = 6,        // Service finished successfully
        NoShow = 7,           // Customer did not arrive
        CancelledByCustomer = 8,
        CancelledByProvider = 9,
        Rejected = 10,        // Provider declined the booking request
        Expired = 11,         // Request/payment window expired
        Rescheduled = 12,     // Booking moved to another date/time
        Refunded = 13,        // Optional: use only if refund is part of booking status
        Failed = 14           // Payment failed, system error, etc.
    }*/

}

public class BookingMap : EntityBaseWithIdCodeMapping<BookingVO>
{
    public BookingMap()
    {
        Map(x => x.Reference).Length(int.MaxValue);
        Map(x => x.Date).Not.Nullable();
        Map(x => x.FromTime).Not.Nullable();
        Map(x => x.ToTime).Not.Nullable();
        Map(x => x.GuestCount).Not.Nullable();
        Map(x => x.Status)
            .CustomType<EnumStringType<BookingVO.BookingStatus>>()
            .Length(50)
            .Not.Nullable();

        Map(x => x.Occasion).Length(100).Not.Nullable();
        //Map(x => x.Dietary).Length(500).Not.Nullable();

        References(x => x.Restaurant, "Restaurant").Not.Nullable().Not.LazyLoad().Index("IX_Bookings_RestaurantId");
        References(x => x.Customer , "Customer").Not.Nullable().Not.LazyLoad().Index("IX_Bookings_CustomerId");
        References(x => x.BookingTable , "BookingTable").Not.LazyLoad().Cascade.None();
    }
}
