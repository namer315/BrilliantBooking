using Data.VO;
using NHibernate;

namespace Data.Extensions;

public static class RepositoryExtensions
{
    public static async Task<string> GetLastCode<T>(this NHibernateRepository repository , string whereCondition = "" , string internalWhereCondition = "" , string leftjoin = "") where T : EntityBaseWithCode
    {
        string internalLeftjoin = leftjoin.Replace("entity" , "entity2");
        string entityName = typeof(T).Name.Replace("Proxy" , "");
        string query = $@"
            SELECT MAX(entity.Code)
			FROM {entityName} as entity
                {leftjoin}
            WHERE entity.Number = (SELECT MAX ( entity2.Number ) FROM {entityName} as entity2 {internalLeftjoin} {internalWhereCondition})
                {whereCondition}
			";

        IQuery q = NHibernateRepository.Session.CreateQuery(query)
        .SetReadOnly(true)
        .SetMaxResults(1);

        string? lastCode = await q.UniqueResultAsync<string?>();

        return lastCode ?? "0";
    }
    public static async Task<bool> IsCodeExist<T>(this NHibernateRepository repository , T t , string whereCondition = "" , string leftjoin = "" ) where T : EntityBaseWithCode
    {
        string entityName = typeof(T).Name;
        IQuery q = NHibernateRepository.Session.CreateQuery($@"
            SELECT 1
			FROM {entityName} as entity
                {leftjoin}
			WHERE
			entity.Code =:code
			AND entity.Id <> :id
            {whereCondition}
			")
        .SetParameter("code" , t.Code)
        .SetParameter("id" , t.Id)
        .SetReadOnly(true);

        int? isExist = await q.UniqueResultAsync<int?>();

        return isExist.HasValue;
    }

    public static async Task<long> GetLastNumber<T>(this NHibernateRepository repository , string whereCondition = "" , string leftjoin = "") where T : EntityBaseWithCode
    {
        string entityName = typeof(T).Name.Replace("Proxy" , "");
        IQuery q = NHibernateRepository.Session.CreateQuery($@"
            SELECT MAX(entity.Number)
			FROM {entityName} as entity
                {leftjoin}
                {whereCondition}
			")
        .SetReadOnly(true);
        long? lastNumber = await q.UniqueResultAsync<long?>();
    
        return lastNumber ?? 0L;
    }
    public static async Task<bool> IsNumberExist<T>(this NHibernateRepository repository ,T t , string whereCondition = "" , string leftjoin = "") where T : EntityBaseWithCode
    {
        string entityName = typeof(T).Name;
        IQuery q = NHibernateRepository.Session.CreateQuery($@"
            SELECT 1
			FROM {entityName} as entity
                {leftjoin}
			WHERE
			entity.Number =:number
			AND entity.Id <> :id
            {whereCondition}
			")
            .SetParameter("number" , t.Number)
            .SetParameter("id" , t.Id)
            .SetReadOnly(true);

        int? isExist = await q.UniqueResultAsync<int?>();

        return isExist.HasValue;
    }
}
