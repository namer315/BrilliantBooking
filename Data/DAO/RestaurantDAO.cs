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

        return await q.UniqueResultAsync<RestaurantVO>();
    }

    public async Task<IList<RestaurantVO>> GetRestaurantListBy(RestaurantSH search)
    {
        string where = "WHERE owner.Id = :ownerId";
        IList<(string , object)> parameters = new List<(string , object)>()
        {
            ("ownerId" , UserSessionManager.CurrentUser.Owner.Id)
        };
        if (!string.IsNullOrEmpty(search.Text)) {
            where = $" AND restaurant.Name like :text";
            parameters.Add(("text" , $"%{search.Text}%"));
        }
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
