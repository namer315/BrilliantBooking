using NHibernate;

namespace Data;

public sealed class NHibernateRepository
{
    private readonly NHibernateSessionFactory _sessionFactory;

    public NHibernateRepository(NHibernateSessionFactory sessionFactory)
    {
        ArgumentNullException.ThrowIfNull(sessionFactory);
        _sessionFactory = sessionFactory;
    }

    public void Persist<T>(T entity) where T : class
    {
        ArgumentNullException.ThrowIfNull(entity);

        using var session = _sessionFactory.OpenSession();
        using var transaction = session.BeginTransaction();

        try
        {
            session.Persist(entity);
            transaction.Commit();
        }
        catch
        {
            if (transaction.IsActive)
            {
                transaction.Rollback();
            }

            throw;
        }
    }

    public T Merge<T>(T entity) where T : class
    {
        ArgumentNullException.ThrowIfNull(entity);

        using var session = _sessionFactory.OpenSession();
        using var transaction = session.BeginTransaction();

        try
        {
            var mergedEntity = session.Merge(entity);
            transaction.Commit();
            return mergedEntity;
        }
        catch
        {
            if (transaction.IsActive)
            {
                transaction.Rollback();
            }

            throw;
        }
    }

    public void Delete<T>(T entity) where T : class
    {
        ArgumentNullException.ThrowIfNull(entity);

        using var session = _sessionFactory.OpenSession();
        using var transaction = session.BeginTransaction();

        try
        {
            session.Delete(entity);
            transaction.Commit();
        }
        catch
        {
            if (transaction.IsActive)
            {
                transaction.Rollback();
            }

            throw;
        }
    }

    public IReadOnlyList<T> Query<T>(
        string hql,
        IReadOnlyDictionary<string, object>? parameters = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hql);

        using var session = _sessionFactory.OpenSession();
        var query = session.CreateQuery(hql);

        if (parameters is not null)
        {
            foreach (var parameter in parameters)
            {
                query.SetParameter(parameter.Key, parameter.Value);
            }
        }

        return query.List<T>().ToArray();
    }
}
