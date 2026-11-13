namespace Data.VO.Restaurant;

/// <summary>
/// Owner account for a restaurant. One owner per restaurant.
/// </summary>
public class RestaurantOwnerVO : EntityBase
{
    public virtual string Name { get; set; } = string.Empty;
    public virtual string? Email { get; set; }
    public virtual string? Phone { get; set; }

    public virtual IList<RestaurantVO> RestaurantList { get; set; } = new List<RestaurantVO>();
}

public class RestaurantOwnerMap : EntityWithIdMapping<RestaurantOwnerVO>
{
    public RestaurantOwnerMap()
    {
        Map(x => x.Name).Length(100).Not.Nullable();
        Map(x => x.Email).Length(256).Nullable();
        Map(x => x.Phone).Length(30).Nullable();

        HasMany(x => x.RestaurantList).KeyColumn("Owner").Cascade.AllDeleteOrphan();
    }
}
