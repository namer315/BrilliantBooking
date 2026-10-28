using FluentNHibernate.Mapping;

namespace Data.VO;

public class RestaurantVO
{
    public virtual Guid Id { get; set; }
    public virtual string Slug { get; set; } = string.Empty;
    public virtual string Name { get; set; } = string.Empty;
    public virtual string Cuisine { get; set; } = string.Empty;
    public virtual string Area { get; set; } = string.Empty;
    public virtual string Description { get; set; } = string.Empty;
    public virtual string Address { get; set; } = string.Empty;
    public virtual string Phone { get; set; } = string.Empty;
    public virtual string Email { get; set; } = string.Empty;
    public virtual string Website { get; set; } = string.Empty;
    public virtual double? Latitude { get; set; }
    public virtual double? Longitude { get; set; }
    public virtual int PriceLevel { get; set; }
}

public class RestaurantVOMap : ClassMap<RestaurantVO>
{
    public RestaurantVOMap()
    {
        Table("RestaurantProfiles");
        Id(x => x.Id).GeneratedBy.GuidComb();
        Map(x => x.Slug).Length(100).Not.Nullable().Unique();
        Map(x => x.Name).Length(200).Not.Nullable();
        Map(x => x.Cuisine).Length(100).Not.Nullable();
        Map(x => x.Area).Length(150).Not.Nullable();
        Map(x => x.Description).Length(2000).Not.Nullable();
        Map(x => x.Address).Length(500).Not.Nullable();
        Map(x => x.Phone).Length(50).Not.Nullable();
        Map(x => x.Email).Length(254).Not.Nullable();
        Map(x => x.Website).Length(500).Not.Nullable();
        Map(x => x.Latitude).Nullable();
        Map(x => x.Longitude).Nullable();
        Map(x => x.PriceLevel).Not.Nullable();
    }
}
