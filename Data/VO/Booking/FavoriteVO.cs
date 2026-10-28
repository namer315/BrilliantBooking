using Data.VO.Customer;
using Data.VO.Restaurant;
using FluentNHibernate.Mapping;

namespace Data.VO.Booking;

/// <summary>
/// Restaurant saved by a customer from the favorites heart button.
/// Source: favoriteIds in index4.html.
/// </summary>
public class FavoriteVO : EntityBase
{
    public virtual CustomerVO Customer { get; set; } = null!;
    public virtual RestaurantVO Restaurant { get; set; } = null!;
}

public class FavoriteMap : EntityWithIdMapping<FavoriteVO>
{
    public FavoriteMap()
    {
        References(x => x.Customer, "Customer").Not.Nullable().Index("IX_Favorites_CustomerId");
        References(x => x.Restaurant, "Restaurant").Not.Nullable();
    }
}
