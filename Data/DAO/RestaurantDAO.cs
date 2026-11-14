using Data.Managers;
using Data.Search;
using Data.VO.Restaurant;
using NHibernate;
using System.Collections;

namespace Data.DAO;

public sealed class RestaurantDAO : NHibernateRepository
{
    public async Task<IList> GetRestaurantBy(string slug)
    {
        IQuery q = Session.CreateQuery(@"
            FROM RestaurantVO restaurant 
            WHERE restaurant.Slug = :slug
			")
            .SetParameter("slug" , slug);

        return await q.ListAsync();
    }
	public async Task<RestaurantVO> GetRestaurantBy(Guid id)
	{
		IQuery q = Session.CreateQuery(@"
			FROM RestaurantVO restaurant 
			WHERE restaurant.Id = :id
			")
			.SetParameter("id" , id)
			.SetMaxResults(1);

		RestaurantVO restaurant = await q.UniqueResultAsync<RestaurantVO>();

		if (restaurant != null)
		{
			// Initialize each bag separately to avoid the "cannot simultaneously fetch multiple bags" error.
			await NHibernateUtil.InitializeAsync(restaurant.FloorList);
			await NHibernateUtil.InitializeAsync(restaurant.PhotoList);
		}

		return restaurant;
	}

    public async Task<IList<RestaurantVO>> GetRestaurantListBy(RestaurantSH search)
    {
        IList<string> conditions = new List<string>();
        IList<(string , object)> parameters = new List<(string , object)>();

        if (search.Owner != null)
        {
            conditions.Add("owner.Id = :ownerId");
            parameters.Add(("ownerId" , search.Owner.Id));
        }

        if (!string.IsNullOrEmpty(search.Text))
        {
            conditions.Add("restaurant.Name like :text");
            parameters.Add(("text" , $"%{search.Text}%"));
        }

        string where = conditions.Count > 0
            ? "WHERE " + string.Join(" AND " , conditions)
            : string.Empty;

        string query = $@"
            SELECT DISTINCT restaurant
            FROM RestaurantVO restaurant
            LEFT JOIN restaurant.Owner owner
            LEFT JOIN FETCH restaurant.PhotoList photo
            {where}
            ORDER BY restaurant.Name
        ";

        return await RunHQLAsync<RestaurantVO>(query, parameters, search);
    }
}
