using FluentNHibernate.Mapping;

namespace Data.VO;

public class ClosureDateVO
{
    public virtual Guid Id { get; set; }
    public virtual Guid RestaurantId { get; set; }
    public virtual DateTime Date { get; set; }
    public virtual string Reason { get; set; } = string.Empty;
}

public class ClosureDateVOMap : ClassMap<ClosureDateVO>
{
    public ClosureDateVOMap()
    {
        Table("RestaurantClosureDates");
        Id(x => x.Id).GeneratedBy.GuidComb();
        Map(x => x.RestaurantId).Not.Nullable();
        Map(x => x.Date).CustomType("Date").Not.Nullable();
        Map(x => x.Reason).Length(500).Not.Nullable();
    }
}
