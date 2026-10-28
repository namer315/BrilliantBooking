using FluentNHibernate.Mapping;

namespace Data.VO.Location;

/// <summary>
/// Dining region offered in the location picker.
/// Source: location-modal buttons in index4.html.
/// </summary>
public class LocationVO : EntityBase
{
    public virtual string Name { get; set; } = string.Empty;
    public virtual string Description { get; set; } = string.Empty;
}

public class LocationMap : EntityWithIdMapping<LocationVO>
{
    public LocationMap()
    {
        Map(x => x.Name).Length(200).Not.Nullable();
        Map(x => x.Description).Length(500).Not.Nullable();
    }
}
