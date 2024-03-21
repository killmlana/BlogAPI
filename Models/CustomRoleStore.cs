using System.Security.Claims;
using BlogAPI.Entities;
using BlogAPI.Helpers;
using IdentityModel;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Models;

public class CustomRoleStore : IRoleClaimStore<Role>
{
    private readonly NHibernateHelper _nHibernateHelper;

    public CustomRoleStore(NHibernateHelper nHibernateHelper)
    {
        _nHibernateHelper = nHibernateHelper;
    }
    public async void Dispose()
    {
        await _nHibernateHelper.Dispose();
    }

    public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
    {
        try
        {
            await _nHibernateHelper.CreateRole(role);
            return IdentityResult.Success;
        }
        catch
        {
            return IdentityResult.Failed();
        }
    }

    public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
    {
        try
        {
            await _nHibernateHelper.UpdateRole(role);
            return IdentityResult.Success;
        }
        catch
        {
            return IdentityResult.Failed();
        }
    }

    public async Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
    {
        try
        {
            await _nHibernateHelper.DeleteRole(role);
            return IdentityResult.Success;
        }
        catch
        {
            return IdentityResult.Failed();
        }
    }

    public async Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
    {
        return await Task.FromResult(role.Id);
    }

    public async Task<string?> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
    {
        return await Task.FromResult(role.Name);
    }

    public async Task SetRoleNameAsync(Role role, string? roleName, CancellationToken cancellationToken)
    {
        if (roleName == null) throw new ArgumentNullException(roleName);
        await _nHibernateHelper.SetRolename(role, roleName);
    }

    public async Task<string?> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
    {
        return await Task.FromResult(role.Name.ToLowerInvariant());
    }

    public async Task SetNormalizedRoleNameAsync(Role role, string? normalizedName, CancellationToken cancellationToken)
    {
        if (normalizedName == null) throw new ArgumentNullException(normalizedName);
        role.Name = normalizedName;
        await UpdateAsync(role, cancellationToken);
    }

    public async Task<Role?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        return await _nHibernateHelper.FindByRoleId(roleId);
    }

    public async Task<Role?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        return await _nHibernateHelper.FindByRoleName(normalizedRoleName);
    }

    public async Task<IList<Claim>> GetClaimsAsync(Role role, CancellationToken cancellationToken)
    {
        return await _nHibernateHelper.GetClaimsFromRole(role);
    }

    public async Task AddClaimAsync(Role role, Claim claim, CancellationToken cancellationToken)
    {
        await _nHibernateHelper.AddClaimToRole(role, claim);
    }

    public async Task RemoveClaimAsync(Role role, Claim claim, CancellationToken cancellationToken)
    {
        await _nHibernateHelper.RemoveClaimFromRole(role, claim);
    }
}