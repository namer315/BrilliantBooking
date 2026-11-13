using Data.VO.Restaurant;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.VO;

public class UserVO : EntityBase
{
    public virtual string Name { get; set; } = string.Empty;
    public virtual string Email { get; set; }
    public virtual string Phone { get; set; }

    public virtual string Password { get; set; }

    public virtual RestaurantOwnerVO Owner { get; set; }

    //public virtual RestaurantVO Restaurant { get; set; } = null!;

    public enum UserType
    {
        Owner,
        Client,
        Management
    }
}

public class UserMap : EntityWithIdMapping<UserVO>
{
    public UserMap()
    {
        Map(x => x.Name).Not.Nullable();
        Map(x => x.Phone).Length(30).Nullable();
        Map(x => x.Email).Not.Nullable().Unique();
        Map(x => x.Password).Not.Nullable();

        References(x => x.Owner , "Owner").Cascade.None().Not.LazyLoad();
    }
}