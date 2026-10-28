using Data.VO.Customer;
using Data.VO.Restaurant;
using FluentNHibernate.Mapping;

namespace Data.VO.Booking;

/// <summary>
/// Table reservation made by a customer.
/// Source: myBookings[] and bookingState in index4.html.
/// </summary>
public class BookingVO : EntityBase
{
    public virtual string Reference { get; set; } = string.Empty;

    public virtual DateOnly Date { get; set; }
    public virtual TimeSpan Time { get; set; }
    public virtual int Guests { get; set; }
    public virtual string TableName { get; set; } = string.Empty;
    public virtual string Status { get; set; } = string.Empty;
    public virtual string Occasion { get; set; } = string.Empty;
    public virtual string Dietary { get; set; } = string.Empty;

    public virtual RestaurantVO Restaurant { get; set; } = null!;
    public virtual CustomerVO Customer { get; set; } = null!;
}

public class BookingMap : EntityWithIdMapping<BookingVO>
{
    public BookingMap()
    {
        Map(x => x.Reference).Length(50).Not.Nullable().Unique();
        Map(x => x.Date).Not.Nullable();
        Map(x => x.Time).Not.Nullable();
        Map(x => x.Guests).Not.Nullable();
        Map(x => x.TableName).Length(100).Not.Nullable();
        Map(x => x.Status).Length(50).Not.Nullable();
        Map(x => x.Occasion).Length(100).Not.Nullable();
        Map(x => x.Dietary).Length(500).Not.Nullable();

        References(x => x.Restaurant, "Restaurant").Not.Nullable().Index("IX_Bookings_RestaurantId");
        References(x => x.Customer, "Customer").Not.Nullable().Index("IX_Bookings_CustomerId");
    }
}
