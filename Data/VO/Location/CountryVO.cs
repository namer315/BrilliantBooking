namespace Data.VO.Location;

/// <summary>
/// Country containing one or more cities in which restaurants operate.
/// </summary>
public class CountryVO : EntityBaseWithNameCode
{
    public virtual string CountryCode { get; set; } = string.Empty;
    public virtual IList<CityVO> CityList { get; set; } = new List<CityVO>();
}

public class CountryMap : EntityBaseWithIdNameCodeMapping<CountryVO>
{
    public CountryMap()
    {
        Map(x => x.CountryCode).Length(10).Not.Nullable().Unique();

        HasMany(x => x.CityList).KeyColumn("Country").Inverse().Cascade.AllDeleteOrphan().LazyLoad();
    }
}
