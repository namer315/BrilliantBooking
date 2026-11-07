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

        booking.GuestCount = 2;
        booking.Date = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3));
        booking.FromTime = DateTime.UtcNow.AddHours(3).TimeOfDay;
        booking.ToTime = DateTime.UtcNow.AddHours(4).TimeOfDay;

        booking.Restaurant = (await new RestaurantBE().GetRestaurantLsitBy(new()))[0];

        return booking;
    }
}
