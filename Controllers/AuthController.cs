using System.IdentityModel.Tokens.Jwt;
using BlogAPI.Entities;
using BlogAPI.Helpers;
using BlogAPI.Models;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NHibernate.Type;

namespace BlogAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly NHibernateHelper _nHibernateHelper;
    private readonly AuthHelper _authHelper;
    private readonly UserManager<User> _manager;
    private readonly SignInManager<User> _signInManager;

    public AuthController(AuthHelper authHelper, NHibernateHelper nHibernateHelper, UserManager<User> manager, SignInManager<User> signInManager)
    {
        _nHibernateHelper = nHibernateHelper;
        _authHelper = authHelper;
        _manager = manager;
        _signInManager = signInManager;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDTO userDto)
    {
        try
        {
            // Register the user
            if (await _manager.FindByNameAsync(userDto.username.ToLowerInvariant()) != null)
                throw new Exception("User already exists.");
            var user = await _authHelper.CreateUser(userDto);
            await _manager.CreateAsync(user);
            var rt = await _authHelper.GenerateRefreshToken(user);
            user.AddRefreshToken(rt);
            await _nHibernateHelper.UpdateUser(user);

            return Ok(new
            {
                RefreshToken = rt.Token,
                AccessToken = await _authHelper.GenerateJwtToken(user)
            });
        }
        catch (Exception ex)
        {
            // Handle registration errors
            throw;
            return BadRequest($"Registration failed: {ex.Message}");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDTO userDto)
    {
        await _authHelper.Login(userDto);
        var user = await _manager.FindByNameAsync(userDto.username.ToLowerInvariant());
        var rt = await _authHelper.GenerateRefreshToken(user);
        user.AddRefreshToken(rt);

        await _nHibernateHelper.UpdateUser(user);

        return Ok(new
        {
            RefreshToken = rt.Token,
            AccessToken = await _authHelper.GenerateJwtToken(user)
        });
    }
    
    [Authorize]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshDTO refreshDto)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(refreshDto.AccessToken);
        var token = refreshDto.RefreshTokenID;

        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Invalid access token.");
        }

        
        var user = await _nHibernateHelper.FindByuserId(userId);  
        
        var rt = await _nHibernateHelper.getRefreshToken(token);
        
        if (!rt.IsActive)
        {
            return Unauthorized("Refresh token has expired.");
        }

        
        var newAccessToken = await _authHelper.GenerateJwtToken(user);

        
        return Ok(new
        {
            AccessToken = newAccessToken,
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshDTO refreshDto)
    {
        var rt = await _nHibernateHelper.getRefreshToken(refreshDto.AccessToken);
        rt.Expires = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        await _nHibernateHelper.UpdateRtAsync(rt);
        return Ok("Logged out.");
    }
    
}
