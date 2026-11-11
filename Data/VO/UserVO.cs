using Data.VO.Restaurant;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.VO;

public class UserVO : EntityBase
{
    public virtual string FullName { get; set; } = string.Empty;
    public virtual string Email { get; set; }
    public virtual string Phone { get; set; }

    public virtual RestaurantVO Restaurant { get; set; } = null!;
}

//public class UserMap : EntityWithIdMapping<UserVO>
//{
//    public UserMap()
//    {
//        Map(x => x.FullName).Length(100).Not.Nullable();
//        Map(x => x.Email).Length(256).Nullable();
//        Map(x => x.Phone).Length(30).Nullable();

//        References(x => x.Restaurant , "Restaurant").Not.Nullable().Unique();
//    }
//}