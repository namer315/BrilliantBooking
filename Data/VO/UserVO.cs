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

