using Business.Common;
using Business.Extensions;
using Data.DAO;
using Data.DTO.Common;
using Data.Exceptions;
using Data.Localization;
using Data.Managers;
using Data.Search;
using Data.VO.Booking;
using Data.VO.Operations;
using Data.VO.Restaurant;
using static Data.VO.Booking.BookingVO;

namespace Business;

public class BookingBE
{
    private BookingDAO _dao = new BookingDAO();

    public async Task ChangeBookingStatus(BookingVO booking , BookingStatus status)
    {
        switch (booking.Status)
        {
            case BookingStatus.Active:
            {
                switch (status)
                {
                    case BookingStatus.CheckedIn:
                        booking = await _dao.GetBookingBy(booking.Id);
                        booking.Status = status;
                        //booking.BookingTable.Status = TableVO.TableStatus.Seated;

                        booking.Arrival = new ArrivalVO();
                        booking.Arrival.Booking = booking;
                        booking.Arrival.ArrivalTable = booking.BookingTable;
                        booking.Arrival.Time = DateTime.UtcNow.TimeOfDay;
                        booking.Arrival.PartySize = booking.GuestCount;
                        booking.Arrival.Status = ArrivalVO.ArrivalStatus.Arrived;

                        booking = await Persist(booking);
                        //await _dao.Update(booking.BookingTable , nameof(TableVO.Status) , booking.BookingTable.Status);
                        break;
                    case BookingStatus.Cancelled:
                        booking.Status = status;
                        await _dao.Update(booking , nameof(BookingVO.Status) , booking.Status);
                        break;
                    default:
                        throw new AppException(
                            ErrorCode.BookingStatusChangeNotAllowed,
                            booking.Status,
                            status);
                }
            }
            break;
            case BookingStatus.CheckedIn:
            {
                switch (status)
                {
                    case BookingStatus.Completed:
                    case BookingStatus.Cancelled:
                    {
                        booking.Status = status;
                        //booking.BookingTable.Status = TableVO.TableStatus.Available;
                        await _dao.Update(booking , nameof(BookingVO.Status) , booking.Status);
                        //await _dao.Update(booking.BookingTable , nameof(TableVO.Status) , booking.BookingTable.Status);
                    }
                        break;
                    default:
                        throw new AppException(
                            ErrorCode.BookingStatusChangeNotAllowed,
                            booking.Status,
                            status);
                }
            }
            break;
            //case BookingStatus.Completed:
            //    break;
            //case BookingStatus.Past:
            //    break;
            //case BookingStatus.Cancelled:

            //    break;
            default:
                throw new AppException(
                    ErrorCode.BookingStatusChangeNotAllowed ,
                    booking.Status ,
                    status);
                break;
        }
    }

    public async Task CheckAvilableTables(BookingVO booking , IList<FloorVO> floorList)
    {
        IList<TableVO> tableList = floorList.SelectMany(x => x.TableList).ToList();
        IList<BookingVO> bookingList = await _dao.GetBookingBy(new BookingSH()
        {
            Date = booking.Date ,
            FromTime = booking.FromTime ,
            ToTime = booking.ToTime
        });

        foreach (BookingVO existBooking in bookingList)
        {
            TableVO table = tableList.FirstOrDefault(x => x.Id == existBooking.BookingTable?.Id);

            if (table is null)
                continue;
            
            table.Status = TableVO.TableStatus.Reserved;            
        }

        foreach (TableVO table in tableList.Where(x => !bookingList.Any(y => y.BookingTable?.Id == x.Id)))
        {
            table.Status = TableVO.TableStatus.Available;
        }
        if (booking.BookingTable is not null
            && tableList.FirstOrDefault(x => x.Id == booking.BookingTable?.Id) is TableVO bookingTable
            && bookingTable.Status == TableVO.TableStatus.Reserved)
        {
            booking.BookingTable = null;
        }
    }

    public async Task<IList<BookingVO>> GetBookingBy(BookingSH search)
    {
        if(!UserSessionManager.IskeyExist
            || (UserSessionManager.CurrentUser.Type == Data.VO.UserVO.UserType.Client && UserSessionManager.CurrentUser.Customer is null)
            || (UserSessionManager.CurrentUser.Type == Data.VO.UserVO.UserType.Owner && search.Resturant is null))
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
        bool isNew = booking.Id == Guid.Empty;
        booking = await _dao.Merge(booking);

        if(isNew)
        {
            EmailDTO email = EmailBuilderBE.CreateBookingEmail(booking);
            //await EmailBE.SendAsync(email);
        }

        return booking;
    }

    private async Task Validation(BookingVO booking)
    {
        await _dao.GetNextUniqueCode(booking);
        await _dao.GetNextUniqueNumber(booking);
    }
}
