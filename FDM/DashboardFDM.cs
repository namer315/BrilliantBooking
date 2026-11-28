using System.Collections.Generic;
using System.Threading.Tasks;
using Business;
using Data.Search;
using Data.VO.Booking;
using Data.VO.Operations;
using Data.VO.Restaurant;

namespace FDM;

public class DashboardFDM
{
    private readonly DashboardBE _be = new();

    public async Task<IList<RestaurantVO>> GetOperationsVenuesAsync()
        => await _be.GetOperationsVenuesAsync();

    //public async Task GetBookingList(DashboardSH search)
    //    => _be.GetResturantDetailsBy(se)

    public async Task<IList<RestaurantVO>> GetResturantList()
        => await new RestaurantBE().GetResturantList();

    public async Task<IList<BookingVO>> GetBookingList(BookingSH search)
        => await new BookingBE().GetBookingBy(search);

    public async Task<IList<TableVO>> GetTableListBy(RestaurantVO? resturant)
    {
        throw new NotImplementedException();
    }

    public async Task<IList<FloorVO>> GetFloorListBy(RestaurantVO resturant)
        => await new FloorBE().GetFloorListBy(resturant);

    public async Task ChangeBookingStatus(BookingVO booking , BookingVO.BookingStatus status)
        => await new BookingBE().ChangeBookingStatus(booking , status);

    public async Task<IList<ArrivalVO>> GetArrivalList()
        => await new ArrivalBE().GetArrivalList();

    public async Task ChangeStatus(ArrivalVO arrival)
        => await new ArrivalBE().ChangeStatus(arrival);
}
