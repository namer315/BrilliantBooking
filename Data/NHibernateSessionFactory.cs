using NHibernate;
using NHibernate.Cfg;

namespace Data;

public sealed class NHibernateSessionFactory : IDisposable
{
    private readonly ISessionFactory _sessionFactory;

    public NHibernateSessionFactory(Action<Configuration> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var configuration = new Configuration();
        configure(configuration);
        _sessionFactory = configuration.BuildSessionFactory();
    }

    public ISession OpenSession() => _sessionFactory.OpenSession();

    public void Dispose() => _sessionFactory.Dispose();
}
