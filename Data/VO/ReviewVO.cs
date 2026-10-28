using FluentNHibernate.Mapping;

namespace Data.VO;

public class ReviewVO
{
    public virtual Guid Id { get; set; }
    public virtual Guid RestaurantId { get; set; }
    public virtual string Author { get; set; } = string.Empty;
    public virtual int Rating { get; set; }
    public virtual string Comment { get; set; } = string.Empty;
    public virtual DateTime CreatedAt { get; set; }
}

public class ReviewVOMap : ClassMap<ReviewVO>
{
    public ReviewVOMap()
    {
        Table("RestaurantReviews");
        Id(x => x.Id).GeneratedBy.GuidComb();
        Map(x => x.RestaurantId).Not.Nullable();
        Map(x => x.Author).Length(150).Not.Nullable();
        Map(x => x.Rating).Not.Nullable();
        Map(x => x.Comment).Length(2000).Not.Nullable();
        Map(x => x.CreatedAt).Not.Nullable();
    }
}
