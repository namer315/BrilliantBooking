using FluentNHibernate.Mapping;

namespace Data.VO;

public class PromotionVO
{
    public virtual Guid Id { get; set; }
    public virtual Guid RestaurantId { get; set; }
    public virtual string Type { get; set; } = string.Empty;
    public virtual string Title { get; set; } = string.Empty;
    public virtual string Body { get; set; } = string.Empty;
    public virtual DateTime? ValidFrom { get; set; }
    public virtual DateTime? ValidTo { get; set; }
}

public class PromotionVOMap : ClassMap<PromotionVO>
{
    public PromotionVOMap()
    {
        Table("RestaurantPromotions");
        Id(x => x.Id).GeneratedBy.GuidComb();
        Map(x => x.RestaurantId).Not.Nullable();
        Map(x => x.Type).Length(40).Not.Nullable();
        Map(x => x.Title).Length(200).Not.Nullable();
        Map(x => x.Body).Length(2000).Not.Nullable();
        Map(x => x.ValidFrom).Nullable();
        Map(x => x.ValidTo).Nullable();
    }
}
