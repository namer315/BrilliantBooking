using System;
using System.Collections.Generic;
using System.Text;
using static Data.VO.Operations.ArrivalVO;

namespace Data.Search;

public class ArrivalSH : Pagination
{
    public IList<ArrivalStatus> StatusList { get; set; }
}
