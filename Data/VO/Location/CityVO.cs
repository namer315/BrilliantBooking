using Data.VO.Restaurant;

namespace Data.VO.Location;

/// <summary>
/// City in which restaurants operate.
/// </summary>
public class CityVO : EntityBaseWithNameCode
{
    public virtual string CityCode { get; set; } = string.Empty;
    public virtual CountryVO Country { get; set; } = null!;
    public virtual IList<RestaurantVO> RestaurantList { get; set; } = new List<RestaurantVO>();
}

public class CityMap : EntityBaseWithIdNameCodeMapping<CityVO>
{
    public CityMap()
    {
        Map(x => x.CityCode).Length(10).Not.Nullable().Unique();

        References(x => x.Country, "Country").Not.Nullable().Index("IX_Cities_CountryId");

        HasMany(x => x.RestaurantList).KeyColumn("City").Inverse().Cascade.Merge().LazyLoad();
    }
}
