using Data.VO.Customer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Search;

public class BookingSH : Pagination
{
    public CustomerVO Customer { get; set; }
}
