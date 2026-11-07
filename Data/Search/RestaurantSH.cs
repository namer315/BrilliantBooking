using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Search;

public class RestaurantSH : SearchBase
{
    public string Cuisine { get; set; }
    public string? Price { get; set; }
}
