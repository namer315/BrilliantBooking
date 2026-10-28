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
}
