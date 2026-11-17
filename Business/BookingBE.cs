using Data.DAO;
using Data.Exceptions;
using Data.Managers;
using Data.Search;
using Data.VO.Booking;
using Data.VO.Restaurant;

namespace Business;

public class BookingBE
{
    private BookingDAO _dao = new BookingDAO();

    public async Task<IList<BookingVO>> GetBookingBy(BookingSH search)
    {
        if(!UserSessionManager.IskeyExist || UserSessionManager.CurrentUser.Customer is null)
            throw new AppException(ErrorCode.AccessDenied);

        return await _dao.GetBookingBy(search);
    }

    public async Task<BookingVO> GetNew(RestaurantVO restaurant)
    {
        if (!UserSessionManager.IskeyExist || UserSessionManager.CurrentUser.Customer is null)
            throw new AppException(ErrorCode.AccessDenied);

        BookingVO booking = new();

        booking.GuestCount = 2;
        booking.Date = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3));
        booking.FromTime = DateTime.UtcNow.AddHours(3).TimeOfDay;
        booking.ToTime = DateTime.UtcNow.AddHours(4).TimeOfDay;
        booking.Customer = UserSessionManager.CurrentUser.Customer;

        //booking.Restaurant = (await new RestaurantBE().GetRestaurantLsitBy(new()))[0];
        booking.Restaurant = await new RestaurantDAO().GetRestaurantBy(restaurant.Id);

        return booking;
    }

    public async Task<BookingVO> Persist(BookingVO booking)
    {
        Validation(booking);

       return await _dao.Merge(booking);
    }

    private void Validation(BookingVO booking)
    {

    }
}
