using FluentNHibernate.Mapping;

namespace Data.VO;

public class RestaurantEventVO
{
    public virtual Guid Id { get; set; }
    public virtual Guid RestaurantId { get; set; }
    public virtual string Title { get; set; } = string.Empty;
    public virtual string Description { get; set; } = string.Empty;
    public virtual DateTime StartsAt { get; set; }
    public virtual DateTime EndsAt { get; set; }
}

public class RestaurantEventVOMap : ClassMap<RestaurantEventVO>
{
    public RestaurantEventVOMap()
    {
        Table("RestaurantEvents");
        Id(x => x.Id).GeneratedBy.GuidComb();
        Map(x => x.RestaurantId).Not.Nullable();
        Map(x => x.Title).Length(200).Not.Nullable();
        Map(x => x.Description).Length(2000).Not.Nullable();
        Map(x => x.StartsAt).Not.Nullable();
        Map(x => x.EndsAt).Not.Nullable();
    }
}
