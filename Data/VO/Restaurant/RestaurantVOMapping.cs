using FluentNHibernate.Mapping;

namespace Data.VO.Restaurant;

public class RestaurantMap : EntityWithIdMapping<RestaurantVO>
{
    public RestaurantMap()
    {
        Map(x => x.Slug).Length(100).Not.Nullable().Unique();
        Map(x => x.Name).Length(200).Not.Nullable();
        Map(x => x.Cuisine).Length(100).Not.Nullable();
        Map(x => x.PriceLevel).Not.Nullable();
        Map(x => x.Rating).Precision(3).Scale(2).Not.Nullable();
        Map(x => x.ReviewsCount).Not.Nullable();
        Map(x => x.Location).Length(150).Not.Nullable();
        Map(x => x.Address).Length(500).Not.Nullable();
        Map(x => x.CrossStreet).Length(300).Not.Nullable();
        Map(x => x.ImageUrl).Length(1000).Not.Nullable();
        Map(x => x.Badge).Length(100).Not.Nullable();
        Map(x => x.Description).Length(2000).Not.Nullable();
        Map(x => x.DressCode).Length(100).Not.Nullable();
        Map(x => x.Parking).Length(200).Not.Nullable();
        Map(x => x.Corkage).Length(200).Not.Nullable();
    }
}
