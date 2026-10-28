using FluentNHibernate.Mapping;

namespace Data.VO;

public class OpeningHourVO
{
    public virtual Guid Id { get; set; }
    public virtual Guid RestaurantId { get; set; }
    public virtual int DayOfWeek { get; set; }
    public virtual TimeSpan OpensAt { get; set; }
    public virtual TimeSpan ClosesAt { get; set; }
}

public class OpeningHourVOMap : ClassMap<OpeningHourVO>
{
    public OpeningHourVOMap()
    {
        Table("RestaurantOpeningHours");
        Id(x => x.Id).GeneratedBy.GuidComb();
        Map(x => x.RestaurantId).Not.Nullable();
        Map(x => x.DayOfWeek).Not.Nullable();
        Map(x => x.OpensAt).Not.Nullable();
        Map(x => x.ClosesAt).Not.Nullable();
    }
}
