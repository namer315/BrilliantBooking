using Business;
using Data.DAO;
using Data.Search;
using Data.VO.Booking;
using Data.VO.Restaurant;

namespace FDM;

public class BookingFDM
{
    private BookingBE _be = new();

    public async Task<IList<BookingVO>> GetBookingBy(BookingSH search)
        => await _be.GetBookingBy(search);

    public async Task<BookingVO> GetBookingBy(Guid id)
        => await new BookingDAO().GetBookingBy(id);

    public async Task<BookingVO> GetNew(RestaurantVO restaurant)
        => await _be.GetNew(restaurant);

    public async Task<BookingVO> Persist(BookingVO booking)
        => await _be.Persist(booking);
}
