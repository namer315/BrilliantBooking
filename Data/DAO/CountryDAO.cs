using Data.VO.Location;
using NHibernate;

namespace Data.DAO;

public sealed class CountryDAO : NHibernateRepository
{
    public async Task<CountryVO> GetById(Guid id)
    {
        IQuery query = Session.CreateQuery(@"
            FROM CountryVO country
            WHERE country.Id = :id")
            .SetParameter("id", id)
            .SetMaxResults(1);

        return await query.UniqueResultAsync<CountryVO>();
    }
    public async Task<CountryVO> GetBy(string countryCode)
    {
        IQuery query = Session.CreateQuery(@"
            FROM CountryVO country
            WHERE country.CountryCode = :countryCode")
            .SetParameter("countryCode" , countryCode)
            .SetMaxResults(1);

        return await query.UniqueResultAsync<CountryVO>();
    }

    public async Task<bool> IsCountryExist(string countryCode)
    {
        IQuery query = Session.CreateQuery(@"
            SELECT 1
            FROM CountryVO country
            WHERE country.CountryCode = :countryCode")
            .SetParameter("countryCode" , countryCode)
            .SetMaxResults(1);

        int? isExist = await query.UniqueResultAsync<int?>();

        return isExist.HasValue;
    }

    public async Task<IList<CountryVO>> GetList(string? text = null)
    {
        string condition = string.IsNullOrWhiteSpace(text)
            ? string.Empty
            : "WHERE lower(country.Name) like :text OR lower(country.Code) like :text";

        IQuery query = Session.CreateQuery($@"
            FROM CountryVO country
            {condition}
            ORDER BY country.Name");

        if (!string.IsNullOrWhiteSpace(text))
        {
            query.SetParameter("text", $"%{text.Trim().ToLowerInvariant()}%");
        }

        return await query.ListAsync<CountryVO>();
    }
}
