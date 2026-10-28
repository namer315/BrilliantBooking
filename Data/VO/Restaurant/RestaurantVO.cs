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
}
