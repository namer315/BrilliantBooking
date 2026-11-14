using Data.VO.Restaurant;

namespace Data.VO.Operations;

public class FloorVO : EntityBaseWithName
{
    public virtual string? Description { get; set; }
    public virtual RestaurantVO Restaurant { get; set; }
    public virtual IList<TableVO> TableList { get; set; } = new List<TableVO>();
}


public class FloorMap : EntityBaseWithNameMapping<FloorVO>
{
    public FloorMap()
    {
        //Map(x => x.Name).Length(100).Not.Nullable();
        Map(x => x.Description).Length(500).Nullable();

        //References(x => x.Restaurant , "Restaurant").Not.Nullable();
        References(x => x.Restaurant , "Restaurant").Not.Nullable();
    }
}

