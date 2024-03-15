using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using BlogAPI.Entities;
using BlogAPI.Models;

namespace BlogAPI.Helpers;

public class AuthHelper
{
    private readonly NHibernateHelper _nHibernateHelper;
    private readonly BcryptHelper _bcryptHelper;

    public AuthHelper(NHibernateHelper nHibernateHelper, BcryptHelper bcryptHelper)
    {
        _nHibernateHelper = nHibernateHelper;
        _bcryptHelper = bcryptHelper;
    }
    public User CreateUser(UserDTO userDto)
    {
        string id = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        id = Regex.Replace(id, "[^0-9a-zA-Z]+", "");
        string hashedPassword = _bcryptHelper.Hash(userDto.password);
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
        if (_bcryptHelper.IsVerified(query, userDto.password))
        {
            return query;
        }
        return null;
    }
}