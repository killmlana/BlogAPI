using System.Security.Authentication;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using BlogAPI.Entities;
using BlogAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NHibernate;

namespace BlogAPI.Helpers;

public class AuthHelper
{
    private readonly NHibernateHelper _nHibernateHelper;
    private readonly BcryptHelper _bcryptHelper;
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthHelper(NHibernateHelper nHibernateHelper, BcryptHelper bcryptHelper, RoleManager<Role> roleManager, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _nHibernateHelper = nHibernateHelper;
        _bcryptHelper = bcryptHelper;
        _roleManager = roleManager;
        _userManager = userManager;
        _signInManager = signInManager;
    }
    public async Task<User> CreateUser(UserDTO userDto)
    {
        string id = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        id = Regex.Replace(id, "[^0-9a-zA-Z]+", "");
        string roleid = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        roleid = Regex.Replace(roleid, "[^0-9a-zA-Z]+", "");
        string hashedPassword = _bcryptHelper.Hash(userDto.password);
        if (await _roleManager.FindByNameAsync("User") == null)
        {
            await _roleManager.CreateAsync(new Role(roleid, "User"));
        }
        var role = await _roleManager.FindByNameAsync("User");
        if (role == null) throw new QueryException("Role not found.");
        var newUser = new User(id, userDto.username, hashedPassword, role, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        return newUser;
    }

    public async Task Login(UserDTO userDto)
    {
        var result = await _signInManager.PasswordSignInAsync(userDto.username, userDto.password, true, false);
        if (!result.Succeeded) throw new AuthenticationException("Wrong username or password.");
    }
}