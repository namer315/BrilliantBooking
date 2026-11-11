using Business;
using Data.DAO;
using Data.Search;
using Data.VO.Restaurant;
using System;
using System.Collections.Generic;
using System.Text;

namespace FDM;

public class RestaurantFDM
{
    private RestaurantBE _be = new();

    public async Task<RestaurantVO> GetRestaurantBy(Guid id)
        => await new RestaurantDAO().GetRestaurantBy(id);

    public async Task<RestaurantVO> GetNew() 
        => await _be.GetNew();

    public async Task<IList<RestaurantVO>> GetRestaurantLsitBy(RestaurantSH search)
        => await _be.GetRestaurantLsitBy(search);

    public async Task<RestaurantVO> Persist(RestaurantVO restaurant)
        => await _be.Persist(restaurant);
}
