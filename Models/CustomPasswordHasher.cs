using BlogAPI.Entities;
using BlogAPI.Helpers;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Models;

public class CustomPasswordHasher : IPasswordHasher<User>
{
    private readonly BcryptHelper _bcryptHelper;

    public CustomPasswordHasher(BcryptHelper bcryptHelper)
    {
        _bcryptHelper = bcryptHelper;
    }

    public string HashPassword(User user, string password)
    {
        return _bcryptHelper.Hash(password);
    }

    public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
    {
        if (_bcryptHelper.IsVerified(user, providedPassword)) return PasswordVerificationResult.Success;
        return PasswordVerificationResult.Failed;
    }
}