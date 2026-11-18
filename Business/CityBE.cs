using Data.DAO;
using Data.VO.Location;

namespace Business;

public class CityBE
{
    private readonly CityDAO _dao = new();

    public Task<CityVO?> GetById(Guid id)
        => _dao.GetById(id);

    public Task<IList<CityVO>> GetByCountry(Guid countryId)
        => _dao.GetByCountry(countryId);

    public Task<IList<CityVO>> GetList(string? text = null)
        => _dao.GetList(text);

    public Task<CityVO> Persist(CityVO city)
    {
        ArgumentNullException.ThrowIfNull(city);
        ArgumentException.ThrowIfNullOrWhiteSpace(city.Name);
        ArgumentNullException.ThrowIfNull(city.Country);

        return _dao.Merge(city);
    }
}
