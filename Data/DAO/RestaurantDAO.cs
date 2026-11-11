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
            where restaurant.Slug = :slug
			")
            .SetParameter("slug" , slug);

        return await q.ListAsync();
    }
    public async Task<RestaurantVO> GetRestaurantBy(Guid id)
    {
        IQuery q = Session.CreateQuery(@"
            FROM RestaurantVO restaurant 
            where restaurant.Id = :id
			")
            .SetParameter("id" , id)
            .SetMaxResults(1);

        return await q.UniqueResultAsync<RestaurantVO>();
    }

    public async Task<IList<RestaurantVO>> GetRestaurantListBy(RestaurantSH search)
    {
        string where = string.Empty;
        IList<(string , object)> parameters = new List<(string , object)>();
        if (!string.IsNullOrEmpty(search.Text)) {
            where = $"where restaurant.Name like :text";
            parameters.Add(("text" , $"%{search.Text}%"));
        }
        string query = $@"
            FROM RestaurantVO restaurant
            LEFT JOIN FETCH restaurant.PhotoList photo
            {where}
            ORDER BY restaurant.Name
        ";

        return await RunHQLAsync<RestaurantVO>(query, parameters, search);
    }
}
