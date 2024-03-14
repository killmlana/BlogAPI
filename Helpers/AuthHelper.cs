using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using BlogAPI.Entities;
using BlogAPI.Models;

namespace BlogAPI.Helpers;

public class AuthHelper
{
    private readonly NHibernateHelper _nHibernateHelper;

    public AuthHelper(NHibernateHelper nHibernateHelper)
    {
        _nHibernateHelper = nHibernateHelper;
    }
    public User CreateUser(UserDTO userDto)
    {
        string id = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        id = Regex.Replace(id, "[^0-9a-zA-Z]+", "");
        string hashedPassword = BcryptHelper.Hash(userDto.password);
        int role = 10;
        var newUser = new User(id, userDto.username, hashedPassword, role, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        return newUser;
    }

    public User? Login(UserDTO userDto)
    {
        //opening Nhibernate session :3
        var session = _nHibernateHelper.OpenSession();
        var query = session.Query<User>().FirstOrDefault(e => e.Username == userDto.username);
        if (query == null) return null;
        if (BcryptHelper.IsVerified(query, userDto))
        {
            return query;
        }
        return null;
    }
}