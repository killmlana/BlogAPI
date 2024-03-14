using BlogAPI.Entities;
using BlogAPI.Helpers;

namespace BlogAPI.Models;
using Microsoft.AspNetCore.Identity;

public class UserStore : IUserStore<User>, IUserPasswordStore<User>
{
    private readonly NHibernateHelper _userService;

    public UserStore(NHibernateHelper userService)
    {
        _userService = userService;
    }


    public void Dispose()
    {
        throw new NotImplementedException();
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
        await _userService.SetUsername(user, userName);
    }

    public async Task<string?> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.Username.ToLower());
    }

    public Task SetNormalizedUserNameAsync(User user, string? normalizedName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
    {
        try
        {
            await _userService.DeleteUser(user);
            return IdentityResult.Success;
        }
        catch
        {
            return IdentityResult.Failed();
        }
    }

    public async Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await _userService.FindByuserId(userId);
    }

    public async Task<User?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        return await _userService.FindByName(normalizedUserName);
    }
}