using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using BlogAPI.Entities;
using BlogAPI.Models;
using Microsoft.AspNetCore.Identity;
using NHibernate;

namespace BlogAPI.Helpers;

public class AuthHelper
{
    private readonly NHibernateHelper _nHibernateHelper;
    private readonly BcryptHelper _bcryptHelper;
    private readonly RoleManager<Role> _roleManager;

    public AuthHelper(NHibernateHelper nHibernateHelper, BcryptHelper bcryptHelper, RoleManager<Role> roleManager)
    {
        _nHibernateHelper = nHibernateHelper;
        _bcryptHelper = bcryptHelper;
        _roleManager = roleManager;
    }
    public async Task<User> CreateUser(UserDTO userDto)
    {
        string id = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        id = Regex.Replace(id, "[^0-9a-zA-Z]+", "");
        string Roleid = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        Roleid = Regex.Replace(id, "[^0-9a-zA-Z]+", "");
        string hashedPassword = _bcryptHelper.Hash(userDto.password);
        if (await _roleManager.FindByNameAsync("User") == null)
        {
            await _roleManager.CreateAsync(new Role(Roleid, "User".ToLowerInvariant()));
        }
        var role = await _roleManager.FindByNameAsync("User");
        if (role == null) throw new QueryException("Role not found.");
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