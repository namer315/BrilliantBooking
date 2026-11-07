using Data.VO.Booking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business;

public class BookingBE
{
    public async Task<BookingVO> GetNew()
    {
        BookingVO booking = new();
        booking.Restaurant = (await new RestaurantBE().GetRestaurantLsitBy(new()))[0];

        return booking;
    }
}
