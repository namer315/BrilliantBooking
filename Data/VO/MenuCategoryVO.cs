using FluentNHibernate.Mapping;

namespace Data.VO;

public class MenuCategoryVO
{
    public virtual Guid Id { get; set; }
    public virtual Guid RestaurantId { get; set; }
    public virtual string Name { get; set; } = string.Empty;
    public virtual string Description { get; set; } = string.Empty;
    public virtual int SortOrder { get; set; }
}

public class MenuCategoryVOMap : ClassMap<MenuCategoryVO>
{
    public MenuCategoryVOMap()
    {
        Table("MenuCategories");
        Id(x => x.Id).GeneratedBy.GuidComb();
        Map(x => x.RestaurantId).Not.Nullable();
        Map(x => x.Name).Length(150).Not.Nullable();
        Map(x => x.Description).Length(500).Not.Nullable();
        Map(x => x.SortOrder).Not.Nullable();
    }
}
