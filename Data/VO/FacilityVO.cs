using FluentNHibernate.Mapping;

namespace Data.VO;

public class FacilityVO
{
    public virtual Guid Id { get; set; }
    public virtual Guid RestaurantId { get; set; }
    public virtual string Code { get; set; } = string.Empty;
    public virtual string Label { get; set; } = string.Empty;
    public virtual string Description { get; set; } = string.Empty;
    public virtual int SortOrder { get; set; }
}

public class FacilityVOMap : ClassMap<FacilityVO>
{
    public FacilityVOMap()
    {
        Table("RestaurantFacilities");
        Id(x => x.Id).GeneratedBy.GuidComb();
        Map(x => x.RestaurantId).Not.Nullable();
        Map(x => x.Code).Length(60).Not.Nullable();
        Map(x => x.Label).Length(150).Not.Nullable();
        Map(x => x.Description).Length(500).Not.Nullable();
        Map(x => x.SortOrder).Not.Nullable();
    }
}
