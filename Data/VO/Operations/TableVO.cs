using Data.VO.Restaurant;
using FluentNHibernate.Mapping;

namespace Data.VO.Operations;

/// <summary>
/// Physical table on the restaurant floor plan.
/// Source: Live Operations floorplan in index4.html.
/// </summary>
public class TableVO : EntityBase
{
    public virtual string Name { get; set; } = string.Empty;
    public virtual int Seats { get; set; }
    public virtual string Zone { get; set; } = string.Empty;
    public virtual string Status { get; set; } = string.Empty;
    public virtual RestaurantVO Restaurant { get; set; } = null!;
}

public class TableMap : EntityWithIdMapping<TableVO>
{
    public TableMap()
    {
        Map(x => x.Name).Length(100).Not.Nullable();
        Map(x => x.Seats).Not.Nullable();
        Map(x => x.Zone).Length(100).Not.Nullable();
        Map(x => x.Status).Length(50).Not.Nullable();

        References(x => x.Restaurant, "Restaurant").Not.Nullable().Index("IX_Tables_RestaurantId");
    }
}
