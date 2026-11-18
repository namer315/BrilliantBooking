using System;
using System.Collections.Generic;
using System.Text;

namespace Business;

public class StartBE
{
    public async Task Init()
    {
        try
        {
            await new CountryBE().PersistTopCountriesWithCities();
        }
        catch (Exception ex)
        {

        }
    }
}
