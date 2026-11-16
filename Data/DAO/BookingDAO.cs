using Data.Search;
using Data.VO.Booking;
using Data.VO.Restaurant;

namespace Data.DAO;

public class BookingDAO : NHibernateRepository
{
    public async Task<IList<BookingVO>> GetBookingBy(BookingSH search)
    {
        IList<string> conditions = new List<string>();
        IList<(string , object)> parameters = new List<(string , object)>();

        if (search.Customer != null)
        {
            conditions.Add("customer.Id = :customerId");
            parameters.Add(("customerId" , search.Customer.Id));
        }

        if (!string.IsNullOrEmpty(search.Text))
        {
            conditions.Add("restaurant.Name like :text");
            parameters.Add(("text" , $"%{search.Text}%"));
        }

        string where = conditions.Count > 0
            ? "WHERE " + string.Join(" AND " , conditions)
            : string.Empty;

        string query = $@"
            FROM BookingVO booking
                LEFT JOIN booking.Restaurant restaurant
                LEFT JOIN booking.Customer customer
            {where}
            ORDER BY restaurant.Name
        ";

        return await RunHQLAsync<BookingVO>(query , parameters , search);
    }
}
