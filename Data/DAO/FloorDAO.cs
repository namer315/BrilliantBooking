using Data.VO.Operations;
using Data.VO.Restaurant;
using NHibernate;

namespace Data.DAO;

public class FloorDAO : NHibernateRepository
{
    public async Task<IList<FloorVO>> GetFloorListBy(RestaurantVO restaurant)
    {
        IQuery q = Session.CreateQuery(@"    
            SELECT DISTINCT floor
            FROM FloorVO floor
                LEFT JOIN floor.Restaurant restaurant
                LEFT JOIN FETCH floor.TableList
            WHERE restaurant.Id = :restaurantId
        ")
            .SetParameter("restaurantId" , restaurant.Id);

        return await q.ListAsync<FloorVO>();
    }
}
