namespace Data.VO.Restaurant;

/// <summary>
/// Opening hours for a restaurant on a single day of the week.
/// </summary>
public class RestaurantHourVO : EntityBase
{
    public virtual DayOfWeek Day { get; set; }
    public virtual bool IsClosed { get; set; }
    public virtual TimeOnly? Open { get; set; }
    public virtual TimeOnly? Close { get; set; }

    public virtual RestaurantVO Restaurant { get; set; } = null!;
}

public class RestaurantHourMap : EntityWithIdMapping<RestaurantHourVO>
{
    public RestaurantHourMap()
    {
        Table("RestaurantHours");

        Map(x => x.Day).Not.Nullable();
        Map(x => x.IsClosed).Not.Nullable();
        Map(x => x.Open).Nullable();
        Map(x => x.Close).Nullable();

        References(x => x.Restaurant, "RestaurantId").Not.Nullable().Index("IX_RestaurantHours_RestaurantId");
    }
}
