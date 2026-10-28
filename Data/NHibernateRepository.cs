using NHibernate;

namespace Data;

public class NHibernateRepository
{
    private readonly NHibernateSessionFactory _sessionFactory;

    //public NHibernateRepository(NHibernateSessionFactory sessionFactory)
    //{
    //    ArgumentNullException.ThrowIfNull(sessionFactory);
    //    _sessionFactory = sessionFactory;
    //}

    #region Session management

    // Thread-safe session storage using AsyncLocal
    private static readonly AsyncLocal<ISession> _session = new AsyncLocal<ISession>();
    /// <summary>
    /// Gets or sets the current NHibernate session
    /// </summary>
    public static ISession Session
    {
        get => GetSession();
        set => _session.Value = value;
    }

    /// <summary>
    /// Gets the current session or creates a new one if needed.
    /// Improved for WinForms and async/threaded scenarios.
    /// </summary>
    public static ISession GetSession()
    {
        var sessionFactory = NHibernateSessionFactory.SessionFactory;
        if (sessionFactory is null || sessionFactory.IsClosed)
            return null;

        return _session.Value ??= sessionFactory.OpenSession();
        //var reusable = TryReuseOrReconnect(_session.Value , "Session reconnect failed, creating new session");
        //if (reusable is not null)
        //    return reusable;

        //_session.Value = OpenTrackedSession(sessionFactory);
        //return _session.Value;
    }

    /// <summary>
    /// Gets a dedicated session for an async operation
    /// </summary>
    //public static async Task<ISession> GetSessionForAsyncOperation(CancellationToken cancellationToken = default)
    //{
    //    var sessionFactory = SessionFactoryGenerator.SessionFactory;
    //    if (sessionFactory is null || sessionFactory.IsClosed)
    //        return null;

    //    // Fast path: reuse existing open session without locking
    //    if (_session.Value is { IsOpen: true, IsConnected: true } fastPathSession)
    //        return fastPathSession;

    //    // Ensure we don't create multiple sessions simultaneously
    //    await _sessionInitLock.WaitAsync(cancellationToken).ConfigureAwait(false);
    //    try
    //    {
    //        // Double-check after acquiring lock
    //        var reusable = TryReuseOrReconnect(_session.Value , "Async session reconnect failed");
    //        if (reusable is not null)
    //            return reusable;

    //        _session.Value = OpenTrackedSession(sessionFactory);
    //        return _session.Value;
    //    }
    //    finally
    //    {
    //        _sessionInitLock.Release();
    //    }
    //}

    /// <summary>
    /// Runs an operation using a brand-new, dedicated NHibernate session that is isolated from
    /// the ambient <see cref="Session"/> used by the rest of the current logical call flow.
    /// This is intended for background/periodic work (e.g. WinForms timers) that runs concurrently
    /// with user-initiated operations on the same thread/SynchronizationContext, to avoid sharing
    /// a single ISession/ADO connection between unrelated concurrent operations - which NHibernate
    /// does not support and can result in "Invalid operation. The connection is closed." errors.
    /// The previous ambient session (if any) is restored for the caller once the operation completes.
    /// </summary>
    //public static async Task<T> RunWithDedicatedSessionAsync<T>(Func<Task<T>> operation , CancellationToken cancellationToken = default)
    //{
    //    if (operation is null)
    //        throw new ArgumentNullException(nameof(operation));

    //    var sessionFactory = SessionFactoryGenerator.SessionFactory;
    //    if (sessionFactory is null || sessionFactory.IsClosed)
    //        throw new InvalidOperationException("Session factory is not initialized.");

    //    ISession previousSession = _session.Value;
    //    ISession dedicatedSession = OpenTrackedSession(sessionFactory);

    //    // Swap in the dedicated session for this logical async flow only.
    //    // AsyncLocal changes made here flow to the awaited operation below but do not
    //    // leak back into the caller's ambient ExecutionContext once this method returns.
    //    _session.Value = dedicatedSession;
    //    try
    //    {
    //        return await operation().ConfigureAwait(false);
    //    }
    //    finally
    //    {
    //        _session.Value = previousSession;
    //        try
    //        {
    //            dedicatedSession.Dispose();
    //        }
    //        catch (Exception ex)
    //        {
    //            //CurrentLogger.Instance.Error($"Error disposing dedicated session: {ex.Message}" , ex);
    //        }
    //    }
    //}

    /// <summary>
    /// Runs an operation using a brand-new, dedicated NHibernate session (no result value).
    /// See <see cref="RunWithDedicatedSessionAsync{T}"/> for details.
    /// </summary>
    //public static async Task RunWithDedicatedSessionAsync(Func<Task> operation , CancellationToken cancellationToken = default)
    //{
    //    await RunWithDedicatedSessionAsync(async () =>
    //    {
    //        await operation().ConfigureAwait(false);
    //        return true;
    //    } , cancellationToken).ConfigureAwait(false);
    //}

    #endregion Session management

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
