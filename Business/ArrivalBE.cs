using Data.DAO;
using Data.Search;
using Data.VO.Booking;
using Data.VO.Operations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business;

public class ArrivalBE
{
    private ArrivalDAO _dao = new ArrivalDAO();

    public async Task ChangeStatus(ArrivalVO arrival)
    {
        switch (arrival.Status)
        {
            case ArrivalVO.ArrivalStatus.Arrived:
            {
                arrival.Status = ArrivalVO.ArrivalStatus.Seated;
                await Persist(arrival);

                arrival.Booking.BookingTable.Status = TableVO.TableStatus.Seated;
                await _dao.Update(arrival.Booking.BookingTable , nameof(TableVO.Status) , arrival.Booking.BookingTable.Status);
            }
            break;
            case ArrivalVO.ArrivalStatus.Seated:
                break;
            case ArrivalVO.ArrivalStatus.NoShow:
                break;
            case ArrivalVO.ArrivalStatus.Cancelled:
                break;
        }
    }

    private async Task<ArrivalVO> Persist(ArrivalVO arrival)
    {
        await Persist(arrival);
        return arrival;
    }

    public async Task<IList<ArrivalVO>> GetArrivalList()
    {
        ArrivalSH search = new ArrivalSH();
        search.StatusList = new List<ArrivalVO.ArrivalStatus>()
        {
            ArrivalVO.ArrivalStatus.Arrived,
            ArrivalVO.ArrivalStatus.Seated,
            //ArrivalVO.ArrivalStatus.Cancelled
        };
        return await _dao.GetArrivalList(search);
    }
    public async Task<IList<ArrivalVO>> GetArrivalList(ArrivalSH search)
    {
        return await _dao.GetArrivalList(search);
    }
}
