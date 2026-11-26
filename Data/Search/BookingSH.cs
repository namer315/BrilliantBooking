using Data.VO;
using Data.VO.Booking;
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
    public DateOnly? Date { get; set; }
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
    public TimeSpan? FromTime { get; set; }
    public TimeSpan? ToTime { get; set; }

    public MenuVO.NavigationRoutes? Route { get; set; }

    public IList<BookingVO.BookingStatus> BookingStatusList { get; set; } = new List<BookingVO.BookingStatus>();
}
