using Data.VO.Restaurant;
using FluentNHibernate.Mapping;
using System.ComponentModel.DataAnnotations;

namespace Data.VO.Operations;

/// <summary>
/// Physical table on the restaurant floor plan.
/// Source: Live Operations floorplan in index4.html.
/// </summary>
public class TableVO : EntityBaseWithName
{
    [Required, StringLength(100)]
    public virtual string Name { get; set; } = string.Empty;
    [Range(1 , 50)]
    public virtual int GuestCount { get; set; } = 1;
    public virtual string? Zone { get; set; }

    /// <summary>
    /// read-only to be use at run time
    /// </summary>
    public virtual TableStatus Status { get; set; } = TableStatus.Available;

    public virtual string Description { get; set; }
    public virtual int Capacity { get; set; }
    //public virtual RestaurantVO Restaurant { get; set; } = null!;
    public virtual FloorVO Floor { get; set; }

    public enum TableStatus
    {
        Available,
        Selected,
        Reserved
    }
}

public class TableMap : EntityBaseWithNameMapping<TableVO>
{
    public TableMap()
    {
        //Map(x => x.Name).Length(100).Not.Nullable();
        Map(x => x.GuestCount).Not.Nullable();
        //Map(x => x.Zone).Length(100).Nullable();
        //Map(x => x.Status).CustomType<TableVO.TableStatus>().Not.Nullable();
        Map(x => x.Description).Length(500).Nullable();

        //References(x => x.Restaurant , "Restaurant").Not.Nullable().Index("IX_Tables_RestaurantId");
        References(x => x.Floor , "Floor").Not.Nullable();
    }
}
