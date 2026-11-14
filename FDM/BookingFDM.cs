using Business;
using Data.VO.Booking;
using Data.VO.Restaurant;
using System;
using System.Collections.Generic;
using System.Text;

namespace FDM;

public class BookingFDM
{
    private BookingBE _be = new();

    public async Task<BookingVO> GetNew(RestaurantVO restaurant)
        => await _be.GetNew(restaurant);

    public async Task<string> Persist(BookingVO booking)
        => await _be.Persist(booking);
}
