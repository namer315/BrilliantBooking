using Business;
using System;
using System.Collections.Generic;
using System.Text;

namespace FDM;

public class StartFDM
{
    public async Task Init()
    {
        await new StartBE().Init();
    }
}
