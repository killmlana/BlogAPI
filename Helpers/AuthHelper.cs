using System.Security.Authentication;
using System.Security.Claims;
using BlogAPI.Entities;
using BlogAPI.Models;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using NHibernate;

namespace BlogAPI.Helpers;

public class AuthHelper
{
    private readonly NHibernateHelper _nHibernateHelper;
    private readonly HashHelper _hashHelper;
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthHelper(NHibernateHelper nHibernateHelper, HashHelper hashHelper, RoleManager<Role> roleManager, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _nHibernateHelper = nHibernateHelper;
        _hashHelper = hashHelper;
        _roleManager = roleManager;
        _userManager = userManager;
        _signInManager = signInManager;
    }
    public async Task<User> CreateUser(UserDTO userDto)
    {
        string hashedPassword = _hashHelper.Hash(userDto.password);
        if (await _roleManager.FindByNameAsync("User") == null)
        {
            await _roleManager.CreateAsync(new Role(_nHibernateHelper.GenerateGuid(), "User"));
        }
        var role = await _roleManager.FindByNameAsync("User");
        if (role == null) throw new QueryException("Role not found.");
        await _roleManager.AddClaimAsync(role, new Claim(JwtClaimTypes.Role, role.Name));
        var newUser = new User(
            _nHibernateHelper.GenerateGuid(),
            userDto.username,
            hashedPassword,
            role,
            DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            );
        return newUser;
    }

    public async Task Login(UserDTO userDto)
    {
        var result = await _signInManager.PasswordSignInAsync(
            userDto.username,
            userDto.password,
            true,
            false
            );
        if (!result.Succeeded) throw new AuthenticationException("Wrong username or password.");
    }
}