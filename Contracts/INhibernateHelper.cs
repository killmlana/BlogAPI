using BlogAPI.Entities;

namespace BlogAPI.Contracts;

public interface INhibernateHelper
{
    NHibernate.ISession OpenSession();
    Task SetUsername(User user, string? username);
    Task<User?> FindByuserId(string id);
    Task DeleteUser(User user);
    Task CreateUser(User user);
}