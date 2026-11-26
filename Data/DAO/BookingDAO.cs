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
            SELECT booking
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

        if (search.Date.HasValue)
        {
            conditions.Add("booking.Date = :date");
            parameters.Add(("date" , search.Date.Value));
        }

        if (search.FromTime.HasValue && search.ToTime.HasValue)
        {
            //booking.FromTime BETWEEN :fromTime AND :fromTime
            //OR booking.ToTime BETWEEN: toTime AND :toTime
            conditions.Add(@"(
                booking.FromTime < :toTime
                AND booking.ToTime > :fromTime
                )");
            parameters.Add(("fromTime" , search.FromTime.Value));
            parameters.Add(("toTime" , search.ToTime.Value));
        }
        /*if (search.FromTime.HasValue)
        {
            conditions.Add("booking.FromTime >= :fromTime");
            parameters.Add(("fromTime" , search.FromTime.Value));
        }

        if (search.ToTime.HasValue)
        {
            conditions.Add("booking.ToTime <= :toTime");
            parameters.Add(("toTime" , search.ToTime.Value));
        }*/

        if (!string.IsNullOrEmpty(search.Text))
        {
            conditions.Add("(lower(booking.Reference) like :text "
                + "OR lower(booking.Occasion) like :text "
                + "OR lower(booking.Note) like :text "
                + "OR lower(booking.ConfirmationCode) like :text "
                + "OR lower(customer.Name) like :text "
                + "OR lower(customer.Phone) like :text "
                + "OR lower(customer.Email) like :text "
                + "OR lower(restaurant.Name) like :text "
                + "OR lower(restaurant.Cuisine) like :text "
                + "OR lower(restaurant.Location) like :text "
                + "OR lower(restaurant.Address) like :text "
                + "OR lower(bookingTable.Name) like :text "
                + "OR lower(bookingTable.Zone) like :text)");
            parameters.Add(("text" , $"%{search.Text.Trim().ToLowerInvariant()}%"));
        }

        if (search.FromDate.HasValue)
        {
            conditions.Add("booking.Date >= :fromDate");
            parameters.Add(("fromDate" , search.FromDate.Value));
        }

        if (search.ToDate.HasValue)
        {
            conditions.Add("booking.Date <= :toDate");
            parameters.Add(("toDate" , search.ToDate.Value));
        }
        if (search.Resturant is not null)
        {

            conditions.Add("restaurant.Id = :restaurantId");
            parameters.Add(("restaurantId" , search.Resturant.Id));
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
            ORDER BY booking.Date DESC , booking.FromTime
        ";

        return await RunHQLAsync<BookingVO>(query , parameters , search);
    }
}
