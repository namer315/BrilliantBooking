using Data.VO.Restaurant;
using FluentNHibernate.Mapping;

namespace Data.VO.Menu;

/// <summary>
/// Dish shown in a restaurant's menu highlights.
/// Source: restaurants[].menu in index4.html.
/// </summary>
public class MenuItemVO : EntityBase
{
    public virtual string Name { get; set; } = string.Empty;
    public virtual string Price { get; set; } = string.Empty;
    public virtual string Tag { get; set; } = string.Empty;
    public virtual RestaurantVO Restaurant { get; set; } = null!;
}

public class MenuItemMap : EntityWithIdMapping<MenuItemVO>
{
    public MenuItemMap()
    {
        Map(x => x.Name).Length(200).Not.Nullable();
        Map(x => x.Price).Length(50).Not.Nullable();
        Map(x => x.Tag).Length(100).Not.Nullable();

        References(x => x.Restaurant, "Restaurant").Not.Nullable().Index("IX_MenuItems_RestaurantId");
    }
}
