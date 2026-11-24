using Business;
using Data.DAO;
using Data.Search;
using Data.VO.Booking;
using Data.VO.Operations;
using Data.VO.Restaurant;

namespace FDM;

public class BookingFDM
{
    private BookingBE _be = new();

    public async Task CheckAvilableTables(BookingVO booking , IList<FloorVO> floorList)
        => await _be.CheckAvilableTables(booking , floorList);

    public async Task<IList<BookingVO>> GetBookingBy(BookingSH search)
        => await _be.GetBookingBy(search);

    public async Task<BookingVO> GetBookingBy(Guid id)
        => await new BookingDAO().GetBookingBy(id);

    public async Task<BookingVO> GetNew(Guid restaurantId)
        => await _be.GetNew(restaurantId);

    public async Task<BookingVO> Persist(BookingVO booking)
        => await _be.Persist(booking);
}
