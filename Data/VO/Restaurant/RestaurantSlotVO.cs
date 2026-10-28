namespace Data.VO.Restaurant;

/// <summary>
/// Bookable seating time for a restaurant.
/// Source: restaurants[].slots in index4.html.
/// </summary>
public class RestaurantSlotVO : EntityBase
{
    public virtual TimeSpan Time { get; set; }
    public virtual RestaurantVO Restaurant { get; set; } = null!;
}
