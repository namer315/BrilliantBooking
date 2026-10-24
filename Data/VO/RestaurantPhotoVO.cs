using FluentNHibernate.Mapping;

namespace Data.VO;

public class RestaurantPhotoVO
{
    public virtual Guid Id { get; set; }
    public virtual Guid RestaurantId { get; set; }
    public virtual string Category { get; set; } = string.Empty;
    public virtual string Url { get; set; } = string.Empty;
    public virtual string AltText { get; set; } = string.Empty;
    public virtual int SortOrder { get; set; }
}

public class RestaurantPhotoVOMap : ClassMap<RestaurantPhotoVO>
{
    public RestaurantPhotoVOMap()
    {
        Table("RestaurantPhotos");
        Id(x => x.Id).GeneratedBy.GuidComb();
        Map(x => x.RestaurantId).Not.Nullable();
        Map(x => x.Category).Length(30).Not.Nullable();
        Map(x => x.Url).Length(1000).Not.Nullable();
        Map(x => x.AltText).Length(300).Not.Nullable();
        Map(x => x.SortOrder).Not.Nullable();
    }
}
