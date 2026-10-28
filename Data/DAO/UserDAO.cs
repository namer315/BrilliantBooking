using Data.VO;

namespace Data.DAO;

public sealed class UserDAO
{
    private const string GetUsersHql = "from UserVO";

    private readonly NHibernateRepository _repository;

    public UserDAO(NHibernateRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    public IList<UserVO> GetUsers() => _repository.Query<UserVO>(GetUsersHql).ToList();
}
