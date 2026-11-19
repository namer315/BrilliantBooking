using Data.Search;
using Data.VO.Booking;
using Data.VO.Restaurant;
using NHibernate;

namespace Data.DAO;

public class BookingDAO : NHibernateRepository
{
    public async Task<BookingVO> GetBookingBy(Guid id)
    {
        IQuery query = Session.CreateQuery(@"
            FROM BookingVO booking
                LEFT JOIN booking.Restaurant restaurant
                LEFT JOIN booking.BookingTable bookingTable
                LEFT JOIN booking.Customer customer
            WHERE booking.Id = :id
        ")
            .SetParameter("id" , id)
            .SetMaxResults(1);

        return await query.UniqueResultAsync<BookingVO>();
    }

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
            SELECT booking
            FROM BookingVO booking
                LEFT JOIN booking.Restaurant restaurant
                LEFT JOIN booking.BookingTable bookingTable
                LEFT JOIN booking.Customer customer
            {where}
            ORDER BY restaurant.Name
        ";

        return await RunHQLAsync<BookingVO>(query , parameters , search);
    }
}
