using Data.VO;
using FluentNHibernate.Mapping;

namespace Data.Mapping;

public class UserVOMap : ClassMap<UserVO>
{
    public UserVOMap()
    {
        Table("UserVO");
        Id(x => x.Id).GeneratedBy.GuidComb();
        Map(x => x.Code).Length(50).Not.Nullable();
        Map(x => x.LocalName).Length(200).Not.Nullable();
        Map(x => x.EnglishName).Length(200).Not.Nullable();
    }
}
