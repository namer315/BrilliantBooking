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
            try
            {
                await Common.EmailBE.LoadConfigAsync();
            }
            catch (FileNotFoundException exF) { }

            await new CountryBE().PersistTopCountriesWithCities();
        }
        catch (Exception ex)
        {
        }
    }
}
