using Data.VO.Customer;
using Data.VO.Operations;
using Data.VO.Restaurant;
using FluentNHibernate.Mapping;
using NHibernate.Type;

namespace Data.VO.Booking;

/// <summary>
/// Table reservation made by a customer.
/// Source: myBookings[] and bookingState in index4.html.
/// </summary>
public class BookingVO : EntityBase
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
        Active,
        Past,
        Cancelled
    }

}

public class BookingMap : EntityWithIdMapping<BookingVO>
{
    public BookingMap()
    {
        Map(x => x.Reference).Length(50).Not.Nullable().Unique();
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
