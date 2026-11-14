using Data.VO.Operations;

namespace Data.VO.Restaurant;

/// <summary>
/// Restaurant profile shown in the explore grid and detail modal.
/// Source: restaurants[] in index4.html.
/// </summary>
public class RestaurantVO : EntityBase
{
    public virtual string Slug { get; set; } = string.Empty;
    public virtual string Name { get; set; } = string.Empty;
    public virtual string Cuisine { get; set; } = string.Empty;
    public virtual int PriceLevel { get; set; }
    public virtual decimal Rating { get; set; }
    public virtual int ReviewsCount { get; set; }
    public virtual string Location { get; set; } = string.Empty;
    public virtual string Address { get; set; } = string.Empty;
    public virtual string CrossStreet { get; set; } = string.Empty;
    public virtual string ImageUrl { get; set; } = string.Empty;
    public virtual string Badge { get; set; } = string.Empty;
    public virtual string Description { get; set; } = string.Empty;
    public virtual string DressCode { get; set; } = string.Empty;
    public virtual string Parking { get; set; } = string.Empty;
    public virtual string Corkage { get; set; } = string.Empty;
    public virtual string Phone { get; set; }
    public virtual string Email { get; set; }
    public virtual int TableCapacity { get; set; }

    public virtual RestaurantOwnerVO Owner { get; set; }
    public virtual IList<RestaurantHourVO> OpeningHourList { get; set; } = new List<RestaurantHourVO>();
    public virtual IList<RestaurantPhotoVO> PhotoList { get; set; } = new List<RestaurantPhotoVO>();
    public virtual IList<TableVO> TableList { get; set; } = new List<TableVO>();
}

public class RestaurantMap : EntityWithIdMapping<RestaurantVO>
{
    public RestaurantMap()
    {
        //Map(x => x.Slug).Length(100).Not.Nullable().Unique();
        Map(x => x.Name).Length(200).Not.Nullable();
        Map(x => x.Cuisine).Length(100).Not.Nullable();
        Map(x => x.PriceLevel).Not.Nullable();
        Map(x => x.Rating).Precision(3).Scale(2).Not.Nullable();
        Map(x => x.ReviewsCount).Not.Nullable();
        Map(x => x.Location).Length(150).Not.Nullable();
        Map(x => x.Address).Length(500).Not.Nullable();
        Map(x => x.CrossStreet).Length(300).Not.Nullable();
        Map(x => x.ImageUrl).Length(1000).Not.Nullable();
        Map(x => x.Badge).Length(100).Not.Nullable();
        Map(x => x.Description).Length(2000).Not.Nullable();
        Map(x => x.DressCode).Length(100).Not.Nullable();
        Map(x => x.Parking).Length(200).Not.Nullable();
        Map(x => x.Corkage).Length(200).Not.Nullable();
        Map(x => x.Phone).Length(30).Nullable();
        Map(x => x.Email).Length(256).Nullable();
        Map(x => x.TableCapacity).Not.Nullable().Default("0");

        References(x => x.Owner, "Owner").Cascade.None();

        HasMany(x => x.OpeningHourList).KeyColumn("Restaurant").Inverse().Cascade.AllDeleteOrphan();
        HasMany(x => x.PhotoList).KeyColumn("Restaurant").OrderBy("Sequence").Inverse().Cascade.AllDeleteOrphan();
        HasMany(x => x.TableList).KeyColumn("Restaurant").Inverse().Cascade.AllDeleteOrphan();
    }
}
