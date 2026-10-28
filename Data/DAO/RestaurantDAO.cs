using Data.VO.Restaurant;
using NHibernate;
using System.Collections;

namespace Data.DAO;

public sealed class RestaurantDAO : NHibernateRepository
{
    public async Task<IList> GetAttendanceEmployeeYears(string code)
    {
        IQuery q = Session.CreateQuery(@"
			SELECT attendanceEmployeeYear.Id,attendanceYear.Code,attendanceYear.LocalName,attendanceYear.EnglishName
            ,employee.Id,employee.Code,employee.LocalName,employee.EnglishName

            FROM  RestaurantVO restaurant 
            where restaurant.Slug = :slug
			")
            .SetParameter("slug" , "%" + code + "%");

        return await q.ListAsync();
    }
}
