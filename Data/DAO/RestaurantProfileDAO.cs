using Data.VO;

namespace Data.DAO;

public sealed class RestaurantProfileDAO
{
    private const string PhotosHql = "from RestaurantPhotoVO photo where photo.RestaurantId = :restaurantId order by photo.SortOrder";
    private const string CategoriesHql = "from MenuCategoryVO category where category.RestaurantId = :restaurantId order by category.SortOrder";
    private const string MenuItemsHql = "from MenuItemVO item where item.RestaurantId = :restaurantId order by item.SortOrder";
    private const string OpeningHoursHql = "from OpeningHourVO hours where hours.RestaurantId = :restaurantId order by hours.DayOfWeek";
    private const string ClosureDatesHql = "from ClosureDateVO closure where closure.RestaurantId = :restaurantId order by closure.Date";
    private const string EventsHql = "from RestaurantEventVO event where event.RestaurantId = :restaurantId order by event.StartsAt";
    private const string ReviewsHql = "from ReviewVO review where review.RestaurantId = :restaurantId order by review.CreatedAt desc";
    private const string FacilitiesHql = "from FacilityVO facility where facility.RestaurantId = :restaurantId order by facility.SortOrder";
    private const string PromotionsHql = "from PromotionVO promotion where promotion.RestaurantId = :restaurantId order by promotion.ValidFrom";

    private readonly NHibernateRepository _repository;

    public RestaurantProfileDAO(NHibernateRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    public RestaurantProfileData? FindBySlug(string slug)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);

        var restaurant = _repository.Query<RestaurantVO>(
            "from RestaurantVO restaurant where restaurant.Slug = :slug",
            new Dictionary<string, object> { ["slug"] = slug }).FirstOrDefault();
        if (restaurant is null)
        {
            return null;
        }

        var parameters = new Dictionary<string, object> { ["restaurantId"] = restaurant.Id };
        return new RestaurantProfileData(
            restaurant,
            _repository.Query<RestaurantPhotoVO>(PhotosHql, parameters),
            _repository.Query<MenuCategoryVO>(CategoriesHql, parameters),
            _repository.Query<MenuItemVO>(MenuItemsHql, parameters),
            _repository.Query<OpeningHourVO>(OpeningHoursHql, parameters),
            _repository.Query<ClosureDateVO>(ClosureDatesHql, parameters),
            _repository.Query<RestaurantEventVO>(EventsHql, parameters),
            _repository.Query<ReviewVO>(ReviewsHql, parameters),
            _repository.Query<FacilityVO>(FacilitiesHql, parameters),
            _repository.Query<PromotionVO>(PromotionsHql, parameters));
    }
}

public sealed record RestaurantProfileData(
    RestaurantVO Restaurant,
    IReadOnlyList<RestaurantPhotoVO> Photos,
    IReadOnlyList<MenuCategoryVO> MenuCategories,
    IReadOnlyList<MenuItemVO> MenuItems,
    IReadOnlyList<OpeningHourVO> OpeningHours,
    IReadOnlyList<ClosureDateVO> ClosureDates,
    IReadOnlyList<RestaurantEventVO> Events,
    IReadOnlyList<ReviewVO> Reviews,
    IReadOnlyList<FacilityVO> Facilities,
    IReadOnlyList<PromotionVO> Promotions);
