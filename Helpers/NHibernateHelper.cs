using System.Security.Claims;
using System.Text.RegularExpressions;
using BlogAPI.Contracts;
using BlogAPI.Entities;
using BlogAPI.Factories;
using BlogAPI.Models;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Linq;
using NHibernate.Tool.hbm2ddl;
using ISession = NHibernate.ISession;

namespace BlogAPI.Helpers;

public class NHibernateHelper 
{

    private readonly SessionFactory _sessionFactory;
    
    public NHibernateHelper(SessionFactory sessionFactory) => _sessionFactory = sessionFactory;
    

    #region UserStore

    public async Task SetUsername(User user, string username, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
        using (var transaction = session.BeginTransaction())
        {
            try
            {
                var userToUpdate = await session.GetAsync<User>(user.Id);
                if (userToUpdate == null)
                {
                    throw new QueryException("No user found.");
                }
                userToUpdate.Name = username;
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

    public async Task<User?> FindByuserId(string id, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
        {
            User? queryUser = await session.GetAsync<User?>(id);
            return queryUser;
        }
    }

    public async Task DeleteUser(User user, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
        using (var transaction = session.BeginTransaction())
        {
            try
            {
                var userToDelete = await session.GetAsync<User>(user.Id);
                if (userToDelete == null) throw new QueryException("User not found.");
                await session.DeleteAsync(userToDelete);
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
                await transaction.RollbackAsync();
            }
        }
    }

    public async Task UpdateUser(User user, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
        using (var transaction = session.BeginTransaction())
        {
            await session.UpdateAsync(user);
            await transaction.CommitAsync();
        }    
    }

    public async Task<User?> FindByUserName(string name, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
        {
            var userToFind = await session.Query<User>()
                .Where(r => r.Name.ToLowerInvariant() == name.ToLowerInvariant())
                .FirstOrDefaultAsync();
            return userToFind;
        }
    }

    public async Task CreateUser(User user, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
        using (var transaction = session.BeginTransaction())
        {
            await session.SaveOrUpdateAsync(user);
            await transaction.CommitAsync();
        }
    } 

    #endregion

    #region RoleStore

    public async Task<Role> GetRoleFromUserAsync(User user, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
        {
            return await session.GetAsync<Role>(user.Role.Id);
        }
    }

    public async Task CreateRole(Role role, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
        try
        {
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

    public async Task UpdateRole(Role role, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
        using (var transaction = session.BeginTransaction())
        {
            await session.UpdateAsync(role);
            await transaction.CommitAsync();
        }
    }

    public async Task DeleteRole(Role role, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
        using (var transaction = session.BeginTransaction())
        {
            try
            {
                var roleToDelete = await session.GetAsync<Role>(role.Id);
                if (roleToDelete == null) throw new QueryException("Role not found.");
                await session.DeleteAsync(roleToDelete);
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
                await transaction.RollbackAsync();
            }
        }
    }

    public async Task SetRolename(Role role, string roleName, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
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

    public async Task<Role?> FindByRoleId(string roleId, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
        {
            var queryRole = await session.GetAsync<Role?>(roleId);
            return queryRole;
        }
    }

    public async Task<Role?> FindByRoleName(string roleName, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
        {
            var roleToFind = await session.Query<Role>()
                .Where(r => r.Name.ToLowerInvariant() == roleName.ToLowerInvariant())
                .ToListAsync();
            return roleToFind.FirstOrDefault();
        }
    }

    #endregion

    #region UserClaimStore

    public async Task<IList<User>> GetUsersForClaim(Claim claim, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
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

    public async Task<bool> UserHasClaim(User user, Claim claim, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
        {
            var userToCheck = await session.GetAsync<User>(user.Id);
            foreach (var customClaim in userToCheck.Claims)
            {
                if (customClaim.ClaimValue.ToLowerInvariant() == claim.Value.ToLowerInvariant() && customClaim.ClaimType.ToLowerInvariant() == claim.Type.ToLowerInvariant()) return true;
            }

            return false;
        }
    }

    public async Task RemoveClaimFromUser(User user, Claim claim, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
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

    public async Task<IList<Claim>> GetClaimsFromUser(User user, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
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

    public async Task ReplaceClaimFromUser(User user, Claim claim, Claim newClaim, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
        if (!await UserHasClaim(user, claim)) throw new NullReferenceException(claim.Value);
        {
            var userToUpdate = await session.GetAsync<User>(user.Id);
            await RemoveClaimFromUser(userToUpdate, claim);
            await AddClaimsToUser(user, new List<Claim>() { newClaim });
        }
    }

    public async Task AddClaimsToUser(User user, IEnumerable<Claim> claims, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
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
                    Id = _sessionFactory.GenerateGuid()
                });
                await session.SaveOrUpdateAsync(customUserClaim);
                await transaction.CommitAsync();
            }
        }
    }

    #endregion

    #region RoleClaimStore

    public async Task<IList<Claim>> GetClaimsFromRole(Role role, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
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

    public async Task AddClaimToRole(Role role, Claim claim, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
        using (var transaction = session.BeginTransaction())
        {
            if (!await RoleHasClaim(role, claim))
            {
                var customRoleClaim = new CustomRoleClaim()
                {
                    Role = role,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value,
                    Id = _sessionFactory.GenerateGuid(),
                    RoleId = role.Id
                };
                await session.SaveOrUpdateAsync(customRoleClaim);
                await transaction.CommitAsync();
            }
        }
    }

    public async Task<bool> RoleHasClaim(Role role, Claim claim, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
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

    public async Task RemoveClaimFromRole(Role role, Claim claim, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
        if (!await RoleHasClaim(role, claim)) throw new NullReferenceException(claim.Value);
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

    public async Task<RefreshToken> GetRefreshToken(String refreshTokenString, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
        {
            var rt = await session.GetAsync<RefreshToken>(refreshTokenString);
            return rt;
        }
    }
    
    public async Task<RefreshToken> GetLatestRtUser(User user)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            return await session.Query<RefreshToken>().OrderByDescending(x => x.Expires).FirstOrDefaultAsync();
        }
    }
    
    public async Task UpdateRtAsync(RefreshToken refreshToken, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }

        using var transaction = session.BeginTransaction();
        await session.UpdateAsync(refreshToken);
        await transaction.CommitAsync();
    }

    public async Task AddRtAsync(RefreshToken refreshToken, ISession? session = null)
    {
        if (session == null)
        {
            session = _sessionFactory.OpenSession();
        }
        using (var transaction = session.BeginTransaction())
        {
            var user = await session.GetAsync<User>(refreshToken.User.Id);
            user.AddRefreshToken(refreshToken);
            await session.SaveOrUpdateAsync(refreshToken);
            await transaction.CommitAsync();
        }
    }
}
            