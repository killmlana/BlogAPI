using System.IdentityModel.Tokens.Jwt;
using BlogAPI.Entities;
using BlogAPI.Factories;
using BlogAPI.Helpers;
using BlogAPI.Models;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly NHibernateHelper _nHibernateHelper;
    private readonly AuthHelper _authHelper;
    private readonly UserManager<User> _manager;
    private readonly SessionFactory _sessionFactory;

    public AuthController(AuthHelper authHelper, NHibernateHelper nHibernateHelper, UserManager<User> manager, SessionFactory sessionFactory)
    {
        _nHibernateHelper = nHibernateHelper;
        _authHelper = authHelper;
        _manager = manager;
        _sessionFactory = sessionFactory;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDTO userDto)
    {
        var session = _sessionFactory.OpenSession();
        // Register the user
        if (await _nHibernateHelper.FindByUserName(userDto.username.ToLowerInvariant(), session) != null)
            throw new Exception("User already exists.");
        var user = await _authHelper.CreateUser(userDto);
        await _nHibernateHelper.CreateUser(user, session);
        var rt = await _authHelper.GenerateRefreshToken(user);
        user.AddRefreshToken(rt);
        await _nHibernateHelper.UpdateUser(user, session);
        session.Dispose();

        return Ok(new
        {
            RefreshToken = rt.Token,
            AccessToken = await _authHelper.GenerateJwtToken(user)
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDTO userDto)
    {
        var session = _sessionFactory.OpenSession();
        await _authHelper.Login(userDto);
        var user = await _nHibernateHelper.FindByUserName(userDto.username.ToLowerInvariant(), session);
        var rt = await _authHelper.GenerateRefreshToken(user);
        var jwt = await _authHelper.GenerateJwtToken(user);
        await _nHibernateHelper.AddRtAsync(rt, session);
        await _nHibernateHelper.UpdateUser(user, session);
        session.Dispose();

        return Ok(new
        {
            RefreshToken = rt.Token,
            AccessToken = jwt
        });
    }
    
    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshDTO refreshDto)
    {
        var session = _sessionFactory.OpenSession();
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(refreshDto.AccessToken);
        var token = refreshDto.RefreshTokenID;

        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Invalid access token.");
        }

        
        var user = await _nHibernateHelper.FindByuserId(userId, session);  
        
        var rt = await _nHibernateHelper.GetRefreshToken(token, session);
        
        if (!rt.IsActive)
        {
            return Unauthorized("Refresh token has expired.");
        }

        
        var newAccessToken = await _authHelper.GenerateJwtToken(user);
        session.Dispose();
        
        return Ok(new
        {
            AccessToken = newAccessToken,
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(string refreshToken)
    {
        var session = _sessionFactory.OpenSession();
        var rt = await _nHibernateHelper.GetRefreshToken(refreshToken, session);
        rt.Expires = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        await _nHibernateHelper.UpdateRtAsync(rt, session);
        session.Dispose();
        return Ok();
    }
    
}
