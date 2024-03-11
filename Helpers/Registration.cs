using System.Text.RegularExpressions;
using BlogAPI.Entities;

namespace BlogAPI.Helpers;

public class Registration
{
    public User CreateUser(string username, string password)
    {
        string id = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        id = Regex.Replace(id, "[^0-9a-zA-Z]+", "");
        string hashedPassword = Authentication.Hash(password);
        int role = 10;
        var newUser = new User(id, username, hashedPassword, role, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        return newUser;
    }
}