using System.Collections.Generic;
using System.Threading.Tasks;
using Business;
using Data.VO.Restaurant;

namespace FDM;

public class DashboardFDM
{
    private readonly DashboardBE _be = new();

    public async Task<IList<RestaurantVO>> GetOperationsVenuesAsync()
        => await _be.GetOperationsVenuesAsync();
}
