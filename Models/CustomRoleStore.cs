using BlogAPI.Entities;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Models;

public class CustomRoleStore : IRoleStore<User>
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public async Task<IdentityResult> CreateAsync(User role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IdentityResult> UpdateAsync(User role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IdentityResult> DeleteAsync(User role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetRoleIdAsync(User role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<string?> GetRoleNameAsync(User role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task SetRoleNameAsync(User role, string? roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<string?> GetNormalizedRoleNameAsync(User role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task SetNormalizedRoleNameAsync(User role, string? normalizedName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}