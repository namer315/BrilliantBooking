using Data.DAO;

namespace ERPBooking.Booking;

public sealed class RestaurantProfileService(RestaurantCatalog catalog, RestaurantProfileDAO? profileDAO = null)
{
    public RestaurantProfile? Find(string slug)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);

        var catalogRestaurant = catalog.Find(slug);
        if (catalogRestaurant is null)
        {
            return null;
        }

        var storedProfile = profileDAO?.FindBySlug(slug);
        return storedProfile is null
            ? RestaurantProfileSamples.Create(catalogRestaurant)
            : CreateStoredProfile(catalogRestaurant, storedProfile);
    }

    private static RestaurantProfile CreateStoredProfile(Restaurant catalogRestaurant, RestaurantProfileData data)
    {
        var openingHours = data.OpeningHours
            .Select(hour => new RestaurantOpeningHour(
                (DayOfWeek)hour.DayOfWeek,
                TimeOnly.FromTimeSpan(hour.OpensAt),
                TimeOnly.FromTimeSpan(hour.ClosesAt)))
            .ToArray();
        var reviews = data.Reviews
            .Select(review => new RestaurantReview(review.Author, review.Rating, review.Comment, review.CreatedAt))
            .ToArray();
        var averageRating = reviews.Length == 0 ? catalogRestaurant.Rating : Math.Round(reviews.Average(review => (decimal)review.Rating), 1);
        var openDays = openingHours.Length == 0
            ? catalogRestaurant.OpenDays
            : openingHours.Select(hour => hour.Day).Distinct().ToArray();
        var openingText = openingHours.Length == 0
            ? catalogRestaurant.OpeningHours
            : string.Join(", ", openingHours.Select(hour => $"{hour.Day}: {hour.OpensAt:HH:mm}-{hour.ClosesAt:HH:mm}"));
        var firstHour = openingHours.FirstOrDefault();
        var restaurant = new Restaurant(
            data.Restaurant.Slug,
            data.Restaurant.Name,
            data.Restaurant.Cuisine,
            data.Restaurant.Area,
            data.Restaurant.PriceLevel,
            averageRating,
            reviews.Length == 0 ? catalogRestaurant.ReviewCount : reviews.Length,
            data.Restaurant.Description,
            data.Restaurant.Address,
            data.Restaurant.Phone,
            openingText,
            string.Join(" ", data.MenuCategories.Select(category => category.Description).Where(description => !string.IsNullOrWhiteSpace(description))),
            data.Photos.FirstOrDefault()?.Url ?? catalogRestaurant.ImageUrl,
            data.Photos.FirstOrDefault()?.AltText ?? catalogRestaurant.ImageAlt,
            catalogRestaurant.Highlight,
            firstHour?.OpensAt ?? catalogRestaurant.OpensAt,
            firstHour?.ClosesAt ?? catalogRestaurant.ClosesAt,
            catalogRestaurant.CoversPerSlot,
            openDays);

        return new RestaurantProfile(
            restaurant,
            data.Restaurant.Email,
            data.Restaurant.Website,
            data.Photos.Select(photo => new RestaurantProfilePhoto(photo.Category, photo.Url, photo.AltText)).ToArray(),
            data.MenuCategories.Select(category => new RestaurantMenuCategory(
                category.Name,
                category.Description,
                data.MenuItems.Where(item => item.CategoryId == category.Id && item.IsAvailable)
                    .Select(item => new RestaurantMenuItem(item.Name, item.Description, item.Price)).ToArray())).ToArray(),
            openingHours,
            data.ClosureDates.Select(closure => new RestaurantClosure(DateOnly.FromDateTime(closure.Date), closure.Reason)).ToArray(),
            data.Events.Select(item => new RestaurantProfileEvent(item.Title, item.Description, item.StartsAt, item.EndsAt)).ToArray(),
            reviews,
            data.Facilities.Select(item => new RestaurantFacility(item.Code, item.Label, item.Description)).ToArray(),
            data.Promotions.Select(item => new RestaurantPromotion(
                item.Type,
                item.Title,
                item.Body,
                item.ValidFrom is null ? null : DateOnly.FromDateTime(item.ValidFrom.Value),
                item.ValidTo is null ? null : DateOnly.FromDateTime(item.ValidTo.Value))).ToArray());
    }
}
