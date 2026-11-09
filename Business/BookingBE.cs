using Data.DAO;
using Data.VO.Booking;

namespace Business;

public class BookingBE
{
    private BookingDAO _dao = new BookingDAO();

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

    public async Task<string> Persist(BookingVO booking)
    {
        Validation(booking);

        var a = await _dao.Merge(booking);
        return "";
    }

    private void Validation(BookingVO booking)
    {

    }
}
