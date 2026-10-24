using Data.VO;

namespace Data.DAO;

public sealed class RestaurantDAO
{
    private const string FindBySlugHql = "from RestaurantVO restaurant where restaurant.Slug = :slug";

    private readonly NHibernateRepository _repository;

    public RestaurantDAO(NHibernateRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    public RestaurantVO? FindBySlug(string slug)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);

        return _repository.Query<RestaurantVO>(FindBySlugHql,
            new Dictionary<string, object> { ["slug"] = slug }).FirstOrDefault();
    }
}
