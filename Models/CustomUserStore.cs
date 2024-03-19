using BlogAPI.Entities;
using BlogAPI.Helpers;
using Microsoft.AspNetCore.Identity;
using Claim = System.Security.Claims.Claim;

namespace BlogAPI.Models;

public class CustomUserStore : IUserPasswordStore<User>, IUserClaimStore<User>
{
    private readonly NHibernateHelper _nHibernateHelper;
    private readonly CustomClaim _customClaim;

    public CustomUserStore(NHibernateHelper userService, CustomClaim customClaim)
    {
        _nHibernateHelper = userService;
        _customClaim = customClaim;
    }

    #region UserStore
    public async void Dispose()
    {
        await _nHibernateHelper.Dispose();
    }

    public async Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.Id);
    }

    public async Task<string?> GetUserNameAsync(User user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.Username);
    }

    public async Task SetUserNameAsync(User user, string? userName, CancellationToken cancellationToken)
    {
        if (userName == null) throw new ArgumentNullException(userName);
        await _nHibernateHelper.SetUsername(user, userName);
    }

    public async Task<string?> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.Username.ToLowerInvariant());
    }

    public async Task SetNormalizedUserNameAsync(User user, string? normalizedName, CancellationToken cancellationToken)
    {
        if (normalizedName == null) throw new ArgumentNullException(normalizedName);
        user.Username = normalizedName;
        await UpdateAsync(user, cancellationToken);
    }

    public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
    {
        try
        {
            await _nHibernateHelper.CreateUser(user);
            return IdentityResult.Success;
        }
        catch (Exception e)
        {
            return IdentityResult.Failed(new IdentityError {Description = "Error occured: " + e});
        }
    }

    public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        try
        {
            await _nHibernateHelper.UpdateUser(user);
            return IdentityResult.Success;
        }
        catch (Exception e)
        {
            return IdentityResult.Failed(new IdentityError {Description = "Error occured: " + e});
        }
    }

    public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
    {
        try
        {
            await _nHibernateHelper.DeleteUser(user);
            return IdentityResult.Success;
        }
        catch
        {
            return IdentityResult.Failed();
        }
    }

    public async Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await _nHibernateHelper.FindByuserId(userId);
    }

    public async Task<User?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        return await _nHibernateHelper.FindByUserName(normalizedUserName);
    }

    public async Task SetPasswordHashAsync(User user, string? passwordHash, CancellationToken cancellationToken)
    {
        if (passwordHash == null) throw new ArgumentNullException(passwordHash);
        user.HashedPassword = await Task.FromResult(passwordHash);
    }

    public async Task<string?> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.HashedPassword);
    }

    public async Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
    {
        if (await Task.FromResult(user.HashedPassword) == null) return false;
        return true;
    }

    #endregion

    public async Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken)
    {
        IList<Claim> list = new List<Claim>();
        foreach (var claim in user.Claims)
        {
            list.Add(claim.ToClaim());
        }

        return list;
    }

    public async Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        foreach (var claim in claims)
        {
            user.AddClaim(new CustomClaim(){ClaimValue = claim.Value, ClaimType = claim.Type, UserId = user.Id});
        } await _nHibernateHelper.UpdateUser(user);
    }

    public async Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
    {
        if (!_nHibernateHelper.UserHasClaim(user, claim)) Console.WriteLine(claim.Value + " not found");
        _nHibernateHelper.RemoveClaimFromUser(user, claim);
        user.AddClaim(new CustomClaim(){ClaimValue = newClaim.Value, ClaimType = newClaim.Type, UserId = user.Id});
        await _nHibernateHelper.UpdateUser(user);
    }

    public async Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        foreach (var claim in claims)
        {
            if (!_nHibernateHelper.UserHasClaim(user, claim)) Console.WriteLine(claim.Value + " not found");
            _nHibernateHelper.RemoveClaimFromUser(user, claim);
        } 
        await _nHibernateHelper.UpdateUser(user);
    }

    public async Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
    {
        return await _nHibernateHelper.GetUsersForClaim(claim);
    }
}