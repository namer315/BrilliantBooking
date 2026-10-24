using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.VO;

public class UserVO
{
    public virtual Guid Id { get; set; }
    //code
    public virtual string Code { get; set; } = string.Empty;
    //Localname
    public virtual string LocalName { get; set; } = string.Empty;
    //Englishname
    public virtual string EnglishName { get; set; } = string.Empty;
}
// map UserVO to UserDAO using NHibernate mapping files or Fluent NHibernate, depending on your preference.
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


