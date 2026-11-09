namespace Data.VO.Restaurant;

/// <summary>
/// Photo associated with a restaurant.
/// </summary>
public class RestaurantPhotoVO : EntityBase
{
    public virtual string Name { get; set; } = string.Empty;
    public virtual byte[] Photo { get; set; }
    public virtual string Url { get; set; } = string.Empty;

    public virtual RestaurantVO Restaurant { get; set; } = null!;
}

public class RestaurantPhotoMap : EntityWithIdMapping<RestaurantPhotoVO>
{
    public RestaurantPhotoMap()
    {
        Map(x => x.Name).Length(260).Not.Nullable();
        Map(x => x.Photo).Length(int.MaxValue).Not.Nullable();
        Map(x => x.Url).Length(1000).Not.Nullable();

        References(x => x.Restaurant, "RestaurantId").Not.Nullable().Index("IX_RestaurantPhotos_RestaurantId");
    }
}
