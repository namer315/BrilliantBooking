using Data.VO.Booking;
using Data.VO.Restaurant;

namespace Data.VO.Operations;

/// <summary>
/// Expected or seated guest party in the shift arrivals stream.
/// Source: Shift Arrivals Stream in index4.html.
/// </summary>
public class ArrivalVO : EntityBase
{
    public virtual TimeSpan Time { get; set; }
    public virtual string GuestName { get; set; } = string.Empty;
    public virtual int PartySize { get; set; }
    public virtual string TableName { get; set; } = string.Empty;
    public virtual string Tags { get; set; } = string.Empty;
    public virtual ArrivalStatus Status { get; set; } = ArrivalStatus.Arrived;
    public virtual RestaurantVO Restaurant { get; set; }
    public virtual BookingVO Booking { get; set; }
    public virtual TableVO Table { get; set; }

    public enum ArrivalStatus
    {
        Expected = 0,
        Arrived = 1,
        Seated = 2,
        NoShow = 3,
        Cancelled = 4
    }

}

public class ArrivalMap : EntityWithIdMapping<ArrivalVO>
{
    public ArrivalMap()
    {
        Map(x => x.Time).Not.Nullable();
        Map(x => x.GuestName).Length(200).Not.Nullable();
        Map(x => x.PartySize).Not.Nullable();
        Map(x => x.TableName).Length(100).Not.Nullable();
        Map(x => x.Tags).Length(200).Not.Nullable();
        Map(x => x.Status);

        //References(x => x.Restaurant , "Restaurant").Not.Nullable().Index("IX_Arrivals_RestaurantId");
        References(x => x.Booking , "Booking").Not.Nullable().Index("IX_Arrivals_BookingId");
    }
}
