using Data.VO.Restaurant;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Search;

public class RestaurantSH : Pagination
{
    public string Cuisine { get; set; }
    public string? Price { get; set; }

    public virtual RestaurantOwnerVO Owner { get; set; }
}
