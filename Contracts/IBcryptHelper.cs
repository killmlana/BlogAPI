using BlogAPI.Entities;

namespace BlogAPI.Contracts;

public interface IBcryptHelper
{
    public string Hash(string password);
    public bool IsVerified(User user, string password);
}