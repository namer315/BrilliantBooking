using System;

namespace Data.Search;
public class SearchBase
{
    public string Text { get; set; }
}

public class Pagination : SearchBase
{
    private int _pageNumber;

    public int PageNumber
    {
        get => _pageNumber;
        set
        {
            if (value < 1)
                throw new ArgumentOutOfRangeException(nameof(value) , "Page number must be greater than 0.");

            _pageNumber = value;
        }
    }
    private int _pageSize;

    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (value < 1)
                throw new ArgumentOutOfRangeException(nameof(value) , "Page size must be greater than 0.");

            _pageSize = value;
        }
    }
    public long TotalRecords { get; internal set; }

    public long PageCount =>
        TotalRecords == 0 ? 0 : (TotalRecords - 1) / PageSize + 1;

    public Pagination()
    {
        PageNumber = 1;
        PageSize = 10;
    }
    public Pagination(int pageNumber , int pageSize)
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber) , "Page number must be greater than 0.");
        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize) , "Page size must be greater than 0.");

        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public int Offset => (PageNumber - 1) * PageSize;
}

public class SearchBaseWithDate : SearchBase
{
    public DateTime FromDate { get; set; } = DateTime.Now.Date.AddMonths(-1);
    public DateTime ToDate { get; set; } = DateTime.Now.Date.AddDays(1).AddTicks(-1);
}

//public class SearchBaseWithDateBranch : SearchBase
//{
//    public DateTime FromDate { get; set; } = DateTime.Now.Date.AddMonths(-1);
//    public DateTime ToDate { get; set; } = DateTime.Now.Date.AddDays(1).AddTicks(-1);

//    private bool branchIsNull;
//    private BranchVO _branch;

//    public BranchVO Branch
//    {
//        get
//        {
//            if (AppBaseEntity.Instance.UserCurrentStatic.Branch != null)
//            {
//                if (_branch != null && (AppBaseEntity.Instance.UserCurrentStatic.Branch.Id != _branch.Id))
//                    return _branch;
//                else if (branchIsNull)
//                    return _branch;
//                else
//                    return AppBaseEntity.Instance.UserCurrentStatic.Branch;
//            }
//            else
//                return SearchParameterVO.Instance.Branch = _branch;
//        }
//        set
//        {
//            _branch = value;
//            if (_branch == null)
//                branchIsNull = true;
//        }
//    }
//}
