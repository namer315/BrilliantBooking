using FluentNHibernate.Mapping;

namespace Data.VO.Restaurant;

public class RestaurantSlotMap : EntityWithIdMapping<RestaurantSlotVO>
{
    public RestaurantSlotMap()
    {
        Map(x => x.Time).Not.Nullable();

        References(x => x.Restaurant, "Restaurant").Not.Nullable().Index("IX_RestaurantSlots_RestaurantId");
    }
}
