using Data.DAO;
using Data.VO.Operations;
using Data.VO.Restaurant;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business;

public class FloorBE
{
    private FloorDAO _dao = new FloorDAO();
    public async Task<IList<FloorVO>> GetFloorListBy(RestaurantVO resturant)
    {
        return await _dao.GetFloorListBy(resturant);
    }
}
