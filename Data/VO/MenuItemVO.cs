using FluentNHibernate.Mapping;

namespace Data.VO;

public class MenuItemVO
{
    public virtual Guid Id { get; set; }
    public virtual Guid RestaurantId { get; set; }
    public virtual Guid CategoryId { get; set; }
    public virtual string Name { get; set; } = string.Empty;
    public virtual string Description { get; set; } = string.Empty;
    public virtual decimal Price { get; set; }
    public virtual int SortOrder { get; set; }
    public virtual bool IsAvailable { get; set; }
}

public class MenuItemVOMap : ClassMap<MenuItemVO>
{
    public MenuItemVOMap()
    {
        Table("MenuItems");
        Id(x => x.Id).GeneratedBy.GuidComb();
        Map(x => x.RestaurantId).Not.Nullable();
        Map(x => x.CategoryId).Not.Nullable();
        Map(x => x.Name).Length(150).Not.Nullable();
        Map(x => x.Description).Length(1000).Not.Nullable();
        Map(x => x.Price).Precision(10).Scale(2).Not.Nullable();
        Map(x => x.SortOrder).Not.Nullable();
        Map(x => x.IsAvailable).Not.Nullable();
    }
}
