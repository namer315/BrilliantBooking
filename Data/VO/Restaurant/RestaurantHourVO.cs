namespace Data.VO.Restaurant;

/// <summary>
/// Opening hours for a restaurant on a single day of the week.
/// </summary>
public class RestaurantHourVO : EntityBase
{
    public virtual DayOfWeek Day { get; set; }
    public virtual bool IsClosed { get; set; }
    public virtual TimeOnly? OpenTime { get; set; }
    public virtual TimeOnly? CloseTime { get; set; }

    public virtual RestaurantVO Restaurant { get; set; } = null!;
}

public class RestaurantHourMap : EntityWithIdMapping<RestaurantHourVO>
{
    public RestaurantHourMap()
    {
        Map(x => x.Day).Not.Nullable();
        Map(x => x.IsClosed).Not.Nullable();
        Map(x => x.OpenTime).Nullable();
        Map(x => x.CloseTime).Nullable();

        References(x => x.Restaurant, "Restaurant").Not.Nullable().Index("IX_RestaurantHours_RestaurantId");
    }
}
