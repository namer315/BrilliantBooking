using Data.DAO;
using Data.Search;
using Data.VO.Operations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business;

public class ArrivalBE
{
    private ArrivalDAO _dao = new ArrivalDAO();

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
