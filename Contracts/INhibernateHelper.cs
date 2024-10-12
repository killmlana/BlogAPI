using BlogAPI.Entities;

namespace BlogAPI.Contracts;

public interface INhibernateHelper
{
    Task SetUsername(User user, string username, ISession session);
    Task<User?> FindByuserId(string id, ISession session);
    Task DeleteUser(User user, ISession session);
    Task CreateUser(User user, ISession session);
    Task CreateRole(Role role, ISession session);
    Task UpdateRole(Role role, ISession session);
    Task DeleteRole(Role role, ISession session);
    Task SetRolename(Role role, string roleName, ISession session);
    Task<Role?> FindByRoleId(string roleId, ISession session);
    Task<Role?> FindByRoleName(string roleName, ISession session);
}