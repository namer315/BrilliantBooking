using Data.VO.Restaurant;
using FluentNHibernate.Mapping;

namespace Data.VO.Review;

/// <summary>
/// Verified diner review shown in the restaurant reviews tab.
/// Source: restaurants[].reviewsList in index4.html.
/// </summary>
public class ReviewVO : EntityBase
{
    public virtual string ReviewerName { get; set; } = string.Empty;
    public virtual string ReviewDate { get; set; } = string.Empty;
    public virtual int Rating { get; set; }
    public virtual string Comment { get; set; } = string.Empty;
    public virtual RestaurantVO Restaurant { get; set; } = null!;
}

public class ReviewMap : EntityWithIdMapping<ReviewVO>
{
    public ReviewMap()
    {
        Map(x => x.ReviewerName).Length(200).Not.Nullable();
        Map(x => x.ReviewDate).Length(50).Not.Nullable();
        Map(x => x.Rating).Not.Nullable();
        Map(x => x.Comment).Length(2000).Not.Nullable();

        References(x => x.Restaurant, "Restaurant").Not.Nullable().Index("IX_Reviews_RestaurantId");
    }
}
