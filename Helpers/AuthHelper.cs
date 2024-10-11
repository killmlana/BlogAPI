using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using BlogAPI.Entities;
using BlogAPI.Models;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NHibernate;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace BlogAPI.Helpers;

public class AuthHelper
{
    private readonly NHibernateHelper _nHibernateHelper;
    private readonly HashHelper _hashHelper;
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthHelper(NHibernateHelper nHibernateHelper, HashHelper hashHelper, RoleManager<Role> roleManager, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
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
    
    public string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtClaimTypes.Id, user.Id),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
            new Claim(JwtClaimTypes.Role, _userManager.GetRolesAsync(user).Result.First()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}