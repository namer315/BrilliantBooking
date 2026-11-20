using Business.Common;
using Business.Extensions;
using Data.DAO;
using Data.DTO.Common;
using Data.Exceptions;
using Data.Localization;
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

    public async Task<BookingVO> GetNew(Guid restaurantId)
    {
        if (!UserSessionManager.IskeyExist || UserSessionManager.CurrentUser.Customer is null)
            throw new AppException(ErrorCode.AccessDenied);

        BookingVO booking = new();

        await _dao.GetNextUniqueCode(booking);
        await _dao.GetNextUniqueNumber(booking);

        //booking.Reference = "TBL-8942-CPT";
        booking.GuestCount = 2;
        booking.Date = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3));
        booking.FromTime = DateTime.UtcNow.AddHours(3).TimeOfDay;
        booking.ToTime = DateTime.UtcNow.AddHours(4).TimeOfDay;
        booking.Customer = UserSessionManager.CurrentUser.Customer;

        //booking.Restaurant = (await new RestaurantBE().GetRestaurantLsitBy(new()))[0];
        booking.Restaurant = await new RestaurantDAO().GetRestaurantBy(restaurantId);

        if(booking.Restaurant is null)
            throw new AppException(ErrorCode.ObjectNotFound , Resources.Restaurant);
        //throw new AppException(ErrorCode.NotFound , $"Restaurant with id '{restaurantId}' not found.");

        return booking;
    }

    public async Task<BookingVO> Persist(BookingVO booking)
    {
        await Validation(booking);
        booking = await _dao.Merge(booking);

        EmailDTO email = EmailBuilderBE.CreateBookingEmail(booking);
        await EmailBE.SendAsync(email);

        return booking;
    }

    private async Task Validation(BookingVO booking)
    {
        await _dao.GetNextUniqueCode(booking);
        await _dao.GetNextUniqueNumber(booking);
    }
}
