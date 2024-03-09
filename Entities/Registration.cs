using System.Text;

namespace BlogAPI.Entities;

public class Registration
{
    public virtual User CreateUser(string username, string password)
    {
        string id = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        string hashedPassword = Authentication.Hash(password);
        int role = 10;
        var newUser = new User(id, username, hashedPassword, role, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        return newUser;
    }
}