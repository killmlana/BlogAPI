using BlogAPI.Entities;
using BlogAPI.Helpers;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Models;

public class CustomPasswordHasher : IPasswordHasher<User>
{
    private readonly HashHelper _hashHelper;

    public CustomPasswordHasher(HashHelper hashHelper)
    {
        _hashHelper = hashHelper;
    }

    public string HashPassword(User user, string password)
    {
        return _hashHelper.Hash(password);
    }

    public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
    {
        if (_hashHelper.IsVerified(user, providedPassword)) return PasswordVerificationResult.Success;
        return PasswordVerificationResult.Failed;
    }
}