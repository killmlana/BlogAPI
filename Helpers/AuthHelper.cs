using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
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
        newUser.AddRefreshToken(await GenerateRefreshToken(newUser));
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
    
    public async Task<string> GenerateJwtToken(User user)
    {
        
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(user.Id))
        {
            throw new ArgumentException("User ID cannot be null or empty.", nameof(user.Id));
        }
        
        var claims = new[]
        {
            new Claim(JwtClaimTypes.Id, user.Id),
            new Claim(JwtClaimTypes.Name, user.Name),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public async Task<RefreshToken> GenerateRefreshToken(User user)
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            var rt = new RefreshToken()
            {
                Token = Convert.ToBase64String(randomNumber),
                Created = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Expires = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds(),
                User = user
            };
            return rt;
        }
        
    }
    
}