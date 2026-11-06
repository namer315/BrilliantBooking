namespace Data.VO.Restaurant;

/// <summary>
/// Photo associated with a restaurant.
/// </summary>
public class RestaurantPhotoVO : EntityBase
{
    public virtual string FileName { get; set; } = string.Empty;
    public virtual string Url { get; set; } = string.Empty;

    public virtual RestaurantVO Restaurant { get; set; } = null!;
}

public class RestaurantPhotoMap : EntityWithIdMapping<RestaurantPhotoVO>
{
    public RestaurantPhotoMap()
    {
        Table("RestaurantPhotos");

        Map(x => x.FileName).Length(260).Not.Nullable();
        Map(x => x.Url).Length(1000).Not.Nullable();

        References(x => x.Restaurant, "RestaurantId").Not.Nullable().Index("IX_RestaurantPhotos_RestaurantId");
    }
}
