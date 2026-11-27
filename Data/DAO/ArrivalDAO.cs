using Data.Search;
using Data.VO.Operations;

namespace Data.DAO;

public class ArrivalDAO : NHibernateRepository
{
    public async Task<IList<ArrivalVO>> GetArrivalList(ArrivalSH search)
    {
        IList<string> conditionList = new List<string>();
        IList<(string , object)> parameterList = new List<(string , object)>();

        if (!string.IsNullOrEmpty(search.Text))
        {
            conditionList.Add("booking.Reference like :text");
            parameterList.Add(("text" , $"%{search.Text}%"));
        }

        if (search.StatusList is { Count: > 0 })
        {
            conditionList.Add("arrival.Status in (:StatusList)");
            parameterList.Add(("StatusList" , search.StatusList));
        }

        string where = conditionList.Count > 0
            ? "WHERE " + string.Join(" AND " , conditionList)
            : string.Empty;

        string query = $@"
            SELECT arrival
            FROM ArrivalVO arrival
                LEFT JOIN arrival.Booking booking
                LEFT JOIN booking.Restaurant restaurant
                LEFT JOIN restaurant.Owner owner
            {where}
            ORDER BY booking.FromTime DESC
        ";

        return await RunHQLAsync<ArrivalVO>(query , parameterList , search);
    }
}
