using Business;
using Data.VO.Booking;
using System;
using System.Collections.Generic;
using System.Text;

namespace FDM;

public class BookingFDM
{
    private BookingBE _be = new();

    public async Task<BookingVO> GetNew()
        => await _be.GetNew();

    public async Task<string> Persist(BookingVO booking)
        => await _be.Persist(booking);
}
