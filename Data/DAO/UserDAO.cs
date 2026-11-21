using Data.VO;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DAO;

public class UserDAO : NHibernateRepository
{
    public async Task<UserVO> GetUserBy(string email /*, string passwordEncrypt*/)
    {
        IQuery query = Session.CreateQuery(@"
            FROM UserVO as user
                LEFT OUTER JOIN FETCH user.Owner
            WHERE 
                user.Email = :username
            ")
            .SetParameter("username" , email)
            //.SetParameter("passwordEncrypt" , passwordEncrypt)
                //AND user.Password = :passwordEncrypt
            .SetMaxResults(1);

        return await query.UniqueResultAsync<UserVO>();
    }
}
