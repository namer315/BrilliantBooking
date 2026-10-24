using Data.VO;
using FluentNHibernate;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;

namespace Data;

public sealed class NHibernateSessionFactory : IDisposable
{
    private readonly ISessionFactory _sessionFactory;

    /// <summary>
    /// Creates a SQL Server session factory using the supplied connection string.
    /// </summary>
    public NHibernateSessionFactory(string connectionString)
        : this(CreateSqlServerConfiguration(connectionString))
    {
    }

    private static Action<Configuration> CreateSqlServerConfiguration(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        return configuration =>
        {
            configuration.DataBaseIntegration(database =>
            {
                database.ConnectionString = connectionString;
                database.Dialect<MsSql2012Dialect>();
                database.Driver<MicrosoftDataSqlClientDriver>();
            });

            var persistenceModel = new PersistenceModel();
            persistenceModel.AddMappingsFromAssembly(typeof(UserVOMap).Assembly);
            persistenceModel.Configure(configuration);
        };
    }

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
