using System.Security.Claims;
using System.Text.RegularExpressions;
using BlogAPI.Contracts;
using BlogAPI.Entities;
using BlogAPI.Models;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Linq;
using NHibernate.Tool.hbm2ddl;
using ISession = NHibernate.ISession;

namespace BlogAPI.Helpers;

public class NHibernateHelper : INhibernateHelper
{

    private readonly ISessionFactory _sessionFactory;

    #region SessionManagement

    public NHibernateHelper()
    {
        _sessionFactory = Fluently.Configure()
            .Database(
                SQLiteConfiguration.Standard
                    .UsingFile("firstProject.db")
            )
            .Mappings(m =>
                m.FluentMappings.AddFromAssemblyOf<Program>())
            .ExposeConfiguration(BuildSchema)
            .BuildSessionFactory();
    } 
    
    private void BuildSchema(Configuration config)
    {
        // this NHibernate tool takes a configuration (with mapping info in)
        // and exports a database schema from it
        new SchemaExport(config)
            .Create(false, false);
    }

    public ISession OpenSession()
    {
        return _sessionFactory.OpenSession();
    }

    public string GenerateGuid()
    {
        string id = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        id = Regex.Replace(id, "[^0-9a-zA-Z]+", "");
        return id;
    }
    
    public ISession GetSession()
    {
        return _sessionFactory.GetCurrentSession();
    }

    public async Task Dispose()
    {
        _sessionFactory.Dispose();
    }

    #endregion

    #region UserStore

    public async Task SetUsername(User user, string username)
    {
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            try
            {
                var userToUpdate = await session.GetAsync<User>(user.Id);
                if (userToUpdate == null)
                {
                    throw new QueryException("No user found.");
                }
                userToUpdate.Username = username;
                await session.SaveOrUpdateAsync(userToUpdate);
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

    public async Task<User?> FindByuserId(string id) // returns null if no user found.
    {
        using (var s = OpenSession())
        {
            User? queryUser = await s.GetAsync<User?>(id);
            return queryUser;
        }
    }

    public async Task DeleteUser(User user)
    {
        using (var session = OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            try
            {
                var userToDelete = await session.GetAsync<User>(user.Id);
                if (userToDelete == null) throw new QueryException("User not found.");
                await session.DeleteAsync(userToDelete);
                await transaction.CommitAsync();
                await session.FlushAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
                await transaction.RollbackAsync();
            }
        }
    }

    public async Task UpdateUser(User user)
    {
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            await session.UpdateAsync(user);
            await session.FlushAsync();
            await transaction.CommitAsync();
        }    
    }

    public async Task<User?> FindByUserName(string name) // returns null when no user is found
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var userToFind = await session.Query<User>()
                .Where(r => r.Username.ToLowerInvariant() == name.ToLowerInvariant())
                .ToListAsync();
            return userToFind.FirstOrDefault();
        }
    }

    public async Task CreateUser(User user)
    {
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            await session.SaveOrUpdateAsync(user);
            await transaction.CommitAsync();
        }
    } 

    #endregion

    #region RoleStore

    public async Task CreateRole(Role role)
    {
        try
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                await session.SaveOrUpdateAsync(role);
                await transaction.CommitAsync();
            }
        } catch (Exception e)
        {
            throw;
        }
    }

    public async Task UpdateRole(Role role)
    {
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            await session.UpdateAsync(role);
            await session.FlushAsync();
            await transaction.CommitAsync();
        }
    }

    public async Task DeleteRole(Role role)
    {
        using (var session = OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            try
            {
                var roleToDelete = await session.GetAsync<Role>(role.Id);
                if (roleToDelete == null) throw new QueryException("Role not found.");
                await session.DeleteAsync(roleToDelete);
                await transaction.CommitAsync();
                await session.FlushAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
                await transaction.RollbackAsync();
            }
        }
    }

    public async Task SetRolename(Role role, string roleName)
    {
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            try
            {
                var roleToUpdate = await session.GetAsync<Role>(role.Id);
                if (roleToUpdate == null)
                {
                    throw new QueryException("No role found.");
                }
                roleToUpdate.Name = roleName;
                await session.SaveOrUpdateAsync(roleName);
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

    public async Task<Role?> FindByRoleId(string roleId) //returns null if no role found.
    {
        using (var s = OpenSession())
        {
            var queryRole = await s.GetAsync<Role?>(roleId);
            return queryRole;
        }
    }

    public async Task<Role?> FindByRoleName(string roleName) //returns null if no role found.
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var roleToFind = await session.Query<Role>()
                .Where(r => r.Name.ToLowerInvariant() == roleName.ToLowerInvariant())
                .ToListAsync();
            return roleToFind.FirstOrDefault();
        }
    }

    #endregion

    #region UserClaimStore

    public async Task<IList<User>> GetUsersForClaim(Claim claim)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var listOfUsers = await session.Query<User>().Where(user =>
                user.Claims.Any(customClaim => 
                    customClaim.ClaimValue.ToLowerInvariant() == claim.Value.ToLowerInvariant()
                                               && 
                    customClaim.ClaimType.ToLowerInvariant() == claim.Type.ToLowerInvariant()
                    )).ToListAsync();
            return listOfUsers;
        }
    }

    public async Task<bool> UserHasClaim(User user, Claim claim)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var userToCheck = await session.GetAsync<User>(user.Id);
            foreach (var customClaim in userToCheck.Claims)
            {
                if (customClaim.ClaimValue.ToLowerInvariant() == claim.Value.ToLowerInvariant() && customClaim.ClaimType.ToLowerInvariant() == claim.Type.ToLowerInvariant()) return true;
            }

            return false;
        }
    }

    public async Task RemoveClaimFromUser(User user, Claim claim)
    {
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())    
        {
            foreach (var customClaim in user.Claims)
            {
                if (customClaim.ClaimValue.ToLowerInvariant() == claim.Value.ToLowerInvariant() && customClaim.ClaimType.ToLowerInvariant() == claim.Type.ToLowerInvariant())
                {
                    user.Claims.Remove(customClaim);
                    await session.SaveOrUpdateAsync(user);
                    await transaction.CommitAsync();
                }
            }
        }
    }

    public async Task<IList<Claim>> GetClaimsFromUser(User user)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var userToGet = await session.GetAsync<User>(user.Id);
            var listOfClaims = new List<Claim>();
            foreach (var customClaim in userToGet.Claims)
            {
                listOfClaims.Add(customClaim.ToClaim());
            }

            return listOfClaims;
        }
    }

    public async Task ReplaceClaimFromUser(User user, Claim claim, Claim newClaim)
    {
        if (!await UserHasClaim(user, claim)) throw new NullReferenceException(claim.Value);
        using (var session = _sessionFactory.OpenSession())
        {
            var userToUpdate = await session.GetAsync<User>(user.Id);
            await RemoveClaimFromUser(userToUpdate, claim);
            await AddClaimsToUser(user, new List<Claim>() { newClaim });
        }
    }

    public async Task AddClaimsToUser(User user, IEnumerable<Claim> claims)
    {
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())    
        {
            var userFromDb = await session.GetAsync<User>(user.Id);
            foreach (var claim in claims)
            {
                if (await UserHasClaim(user, claim)) continue;
                var customUserClaim = (new CustomUserClaim()
                {
                    User = userFromDb,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value,
                    UserId = userFromDb.Id,
                    Id = GenerateGuid()
                });
                await session.SaveOrUpdateAsync(customUserClaim);
                await transaction.CommitAsync();
            }
        }
    }

    #endregion

    #region RoleClaimStore

    public async Task<IList<Claim>> GetClaimsFromRole(Role role)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var roleToGet = await session.GetAsync<Role>(role.Id);
            var listOfClaims = new List<Claim>();
            foreach (var customClaim in roleToGet.Claims)
            {
                listOfClaims.Add(customClaim.ToClaim());
            }

            return listOfClaims;
        }
    }

    public async Task AddClaimToRole(Role role, Claim claim)
    {
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            if (!await RoleHasClaim(role, claim))
            {
                var customRoleClaim = new CustomRoleClaim()
                {
                    Role = role,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value,
                    Id = GenerateGuid(),
                    RoleId = role.Id
                };
                await session.SaveOrUpdateAsync(customRoleClaim);
                await transaction.CommitAsync();
            }
        }
    }

    public async Task<bool> RoleHasClaim(Role role, Claim claim)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var roleToCheck = await session.GetAsync<Role>(role.Id);
            foreach (var customClaim in roleToCheck.Claims)
            {
                if (customClaim.ClaimValue.ToLowerInvariant() == claim.Value.ToLowerInvariant() && customClaim.ClaimType.ToLowerInvariant() == claim.Type.ToLowerInvariant())
                {
                    return true;
                }
            }
            return false;
        }
    }

    public async Task RemoveClaimFromRole(Role role, Claim claim)
    {
        if (!await RoleHasClaim(role, claim)) throw new NullReferenceException(claim.Value);
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())    
        {
            foreach (var customClaim in role.Claims)
            {
                if (!(customClaim.ClaimValue.ToLowerInvariant() == claim.Value.ToLowerInvariant() && customClaim.ClaimType.ToLowerInvariant() == claim.Type.ToLowerInvariant())) continue;
                role.Claims.Remove(customClaim);
                await session.SaveOrUpdateAsync(role);
                await transaction.CommitAsync();
            }
        }
    }

    #endregion
}
            