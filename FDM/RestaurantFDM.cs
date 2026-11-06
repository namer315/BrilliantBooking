using System;
using System.Collections.Generic;
using System.Text;
using Business;
using Data.VO.Restaurant;

namespace FDM;

public class RestaurantFDM
{
    private RestaurantBE _be = new();

    public async Task Persist(RestaurantVO restaurant)
        => await _be.Persist(restaurant);
}
