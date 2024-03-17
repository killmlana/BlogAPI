using BlogAPI.Entities;

namespace BlogAPI.Contracts;

public interface INhibernateHelper
{
    NHibernate.ISession OpenSession();
    Task SetUsername(User user, string username);
    Task<User?> FindByuserId(string id);
    Task DeleteUser(User user);
    Task CreateUser(User user);
    Task CreateRole(Role role);
    Task UpdateRole(Role role);
    Task DeleteRole(Role role);
    Task SetRolename(Role role, string roleName);
    Task<Role?> FindByRoleId(string roleId);
    Task<Role?> FindByRoleName(string roleName);
}