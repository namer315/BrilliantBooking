using Data.DAO;
using Data.Exceptions;
using Data.Localization;
using Data.VO.Operations;
using Data.VO.Restaurant;

namespace Business;

public class FloorBE
{
    private FloorDAO _dao = new FloorDAO();
    public async Task<IList<FloorVO>> GetFloorListBy(RestaurantVO resturant)
    {
        if (resturant is null)
            throw new AppException(ErrorCode.FieldIsRequired, Resources.Restaurant);

        return await _dao.GetFloorListBy(resturant);
    }
}
