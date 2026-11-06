using Data.DAO;
using Data.VO.Restaurant;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business;

public class RestaurantBE
{
    private RestaurantDAO _dao = new();

    public async Task Persist(RestaurantVO restaurant)
    {
        Validation(restaurant);

        await _dao.Persist(restaurant);
    }

    private void Validation(RestaurantVO restaurant)
    {

    }
}
