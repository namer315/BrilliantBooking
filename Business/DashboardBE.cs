using System.Collections.Generic;
using System.Threading.Tasks;
using Data.VO.Restaurant;

namespace Business;

public class DashboardBE
{
    public Task<IList<RestaurantVO>> GetOperationsVenuesAsync()
        => Task.FromResult<IList<RestaurantVO>>(new List<RestaurantVO>());
}
