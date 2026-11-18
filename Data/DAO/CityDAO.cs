using Data.VO.Location;
using NHibernate;

namespace Data.DAO;

public sealed class CityDAO : NHibernateRepository
{
    public async Task<CityVO> GetById(Guid id)
    {
        IQuery query = Session.CreateQuery(@"
            FROM CityVO city
            WHERE city.Id = :id")
            .SetParameter("id", id)
            .SetMaxResults(1);

        return await query.UniqueResultAsync<CityVO>();
    }

    public async Task<CityVO> GetBy(string cityCode, Guid countryId)
    {
        IQuery query = Session.CreateQuery(@"
            FROM CityVO city
            WHERE city.CityCode = :cityCode
              AND city.Country.Id = :countryId
        ")
            .SetParameter("cityCode", cityCode)
            .SetParameter("countryId", countryId)
            .SetMaxResults(1);

        return await query.UniqueResultAsync<CityVO>();
    }
    public async Task<CityVO> GetBy(string cityCode)
    {
        IQuery query = Session.CreateQuery(@"
            FROM CityVO city
            WHERE city.CityCode = :cityCode")
            .SetParameter("cityCode" , cityCode)
            .SetMaxResults(1);

        return await query.UniqueResultAsync<CityVO>();
    }
    public async Task<bool> IsCityExist(string cityCode)
    {
        IQuery query = Session.CreateQuery(@"
            SELECT 1
            FROM CityVO city
            WHERE city.CityCode = :cityCode")
            .SetParameter("cityCode" , cityCode)
            .SetMaxResults(1);

        int? isExist = await query.UniqueResultAsync<int?>();

        return isExist.HasValue;
    }

    public async Task<IList<CityVO>> GetByCountry(Guid countryId)
    {
        IQuery query = Session.CreateQuery(@"
            FROM CityVO city
            WHERE city.Country.Id = :countryId
            ORDER BY city.Name")
            .SetParameter("countryId", countryId);

        return await query.ListAsync<CityVO>();
    }

    public async Task<IList<CityVO>> GetList(string? text = null)
    {
        string condition = string.IsNullOrWhiteSpace(text)
            ? string.Empty
            : "WHERE lower(city.Name) like :text";

        IQuery query = Session.CreateQuery($@"
            FROM CityVO city
            {condition}
            ORDER BY city.Name");

        if (!string.IsNullOrWhiteSpace(text))
        {
            query.SetParameter("text", $"%{text.Trim().ToLowerInvariant()}%");
        }

        return await query.ListAsync<CityVO>();
    }
}
