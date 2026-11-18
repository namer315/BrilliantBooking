using Business.Extensions;
using Data.DAO;
using Data.VO.Location;

namespace Business;

public class CountryBE
{
    private readonly CountryDAO _dao = new();
    private readonly CityDAO _cityDAO = new();

    public Task<CountryVO?> GetById(Guid id)
        => _dao.GetById(id);

    public Task<IList<CountryVO>> GetList(string text = "")
        => _dao.GetList(text);

    /// <summary>
    /// Creates the initial country and city list used by the location selector.
    /// The returned objects are not persisted until <see cref="Persist"/> is called.
    /// </summary>
    private IList<CountryVO> CreateTopCountriesWithCities()
    {
        return new List<CountryVO>
        {
            CreateCountry("United Arab Emirates", "AE",
                ("Dubai", "DXB"),
                ("Abu Dhabi", "AUH"),
                ("Sharjah", "SHJ")),

            CreateCountry("Saudi Arabia", "SA",
                ("Riyadh", "RUH"),
                ("Jeddah", "JED"),
                ("Mecca", "MAK"),
                ("Medina", "MED"),
                ("Dammam", "DMM")),

            CreateCountry("Egypt", "EG",
                ("Cairo", "CAI"),
                ("Alexandria", "ALY"),
                ("Giza", "GIZ"),
                ("Sharm El Sheikh", "SSH"),
                ("Luxor", "LXR")),

            CreateCountry("South Africa", "ZA",
                ("Johannesburg", "JNB"),
                ("Cape Town", "CPT"),
                ("Durban", "DUR"),
                ("Pretoria", "PRY"),
                ("Port Elizabeth", "PLZ"))
        };
    }

    public async Task<IList<CountryVO>> PersistTopCountriesWithCities()
    {
        IList<CountryVO> countryList = CreateTopCountriesWithCities();
        IList<CountryVO> persistedCountries = new List<CountryVO>();

        foreach (CountryVO country in countryList)
        {
            persistedCountries.Add(await Persist(country));
        }

        return persistedCountries;
    }

    private async Task<CountryVO> Persist(CountryVO country)
    {
        ArgumentNullException.ThrowIfNull(country);
        ArgumentException.ThrowIfNullOrWhiteSpace(country.Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(country.CountryCode);

        await _dao.GetNextUniqueCode(country);
        await _dao.GetNextUniqueNumber(country);

        return await  PersistCountryAndCities(country);
    }

    private async Task<CountryVO> PersistCountryAndCities(CountryVO country)
    {
        CountryVO existingCountry = await _dao.GetBy(country.CountryCode);

        if (existingCountry is null)
        {
            HashSet<string> cityCodes = new(StringComparer.OrdinalIgnoreCase);

            foreach (CityVO city in country.CityList)
            {
                if (!cityCodes.Add(city.CityCode))
                {
                    continue;
                }

                await _dao.GetNextUniqueCode(city);
                await _dao.GetNextUniqueNumber(city);

                city.Country = country;

                //await _cityDAO.Persist(city);
            }

            country.CityList = country.CityList
                .Where(city => cityCodes.Contains(city.CityCode))
                .GroupBy(city => city.CityCode , StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .ToList();

            return await _dao.Merge(country);
        }
        else
        {

            HashSet<string> existingCityCodes = existingCountry.CityList
                .Select(city => city.CityCode)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (CityVO city in country.CityList)
            {
                if (!existingCityCodes.Add(city.CityCode))
                {
                    continue;
                }

                CityVO existingCity = await _cityDAO.GetBy(city.CityCode , existingCountry.Id);

                if (existingCity is null)
                {
                    await _dao.GetNextUniqueCode(city);
                    await _dao.GetNextUniqueNumber(city);

                    city.Country = existingCountry;
                    existingCountry.CityList.Add(city);
                }
                else if (country.CityList.FirstOrDefault(x => x.Id == existingCity.Id) is CityVO e_city)
                {
                    await _dao.GetNextUniqueCode(e_city);
                    await _dao.GetNextUniqueNumber(e_city);
                }
            }

            return await _dao.Merge(existingCountry);
        }
    }

    private CountryVO CreateCountry(
        string name,
        string countryCode,
        params (string Name, string Code)[] cities)
    {
        CountryVO country = new()
        {
            Name = name,
            CountryCode = countryCode
        };

        foreach ((string cityName, string cityCode) in cities)
        {
            country.CityList.Add(new CityVO
            {
                Name = cityName,
                CityCode = cityCode,
                Country = country
            });
        }

        return country;
    }
}
