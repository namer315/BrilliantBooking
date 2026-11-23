using Data.VO.Customer;
using Data.VO.Restaurant;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Search;

public class BookingSH : Pagination
{
    public CustomerVO Customer { get; set; }
    public RestaurantVO Resturant { get; set; }
}
