using BlogAPI.Entities;

namespace BlogAPI.Contracts;

public interface INhibernateHelper
{
    NHibernate.ISession OpenSession();
    Task SetUsername(User user, string? username, NHibernate.ISession? session = null);
    Task<User?> FindByuserId(string id, NHibernate.ISession? s = null);
    Task DeleteUser(User user, NHibernate.ISession? session = null);
}