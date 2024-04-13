using System.Configuration;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BlogAPI.Entities;
using BlogAPI.Models;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NHibernate;

namespace BlogAPI.Helpers;

public class AuthHelper
{
    private readonly NHibernateHelper _nHibernateHelper;
    private readonly HashHelper _hashHelper;
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthHelper(NHibernateHelper nHibernateHelper, HashHelper hashHelper, RoleManager<Role> roleManager,
        UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
    {
        _nHibernateHelper = nHibernateHelper;
        _hashHelper = hashHelper;
        _roleManager = roleManager;
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
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

    public async Task<User> Login(UserDTO userDto) //returns the user signed in
    {
        var result = await _signInManager.PasswordSignInAsync(
            userDto.username,
            userDto.password,
            true,
            false
        ); 
        if (!result.Succeeded) throw new AuthenticationException("Wrong username or password.");
        return await _userManager.FindByNameAsync(userDto.username);
    }

    public async Task<string> GenerateJwt(User user)
    {
        var tokenHandler = new JsonWebTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["JWT:Signing-Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtClaimTypes.Id, user.Id),
                new Claim(JwtClaimTypes.Name, user.Username),
                new Claim(JwtClaimTypes.Role, user.Role.Id)
            }),
            Expires = DateTime.UtcNow.AddHours(6),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha384)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return token;
    }
}