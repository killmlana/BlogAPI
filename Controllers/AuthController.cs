using System.IdentityModel.Tokens.Jwt;
using BlogAPI.Entities;
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

            return Ok(new
            {
                RefreshToken = _authHelper.GenerateRefreshToken(user).Result.Token,
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
        return Ok(new
        {
            RefreshToken = _authHelper.GenerateRefreshToken(user).Result.Token,
            AccessToken = await _authHelper.GenerateJwtToken(user)
        });
    }
    
    [Authorize]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshDTO refreshDto)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(refreshDto.AccessToken);

        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Invalid access token.");
        }

        // 2. Fetch User from the Database
        var user = await _nHibernateHelper.FindByuserId(userId);  // Assuming this method gets the user by ID

        // 3. Check if RefreshTokenID exists for the user
        var rt = await _nHibernateHelper.getRefreshToken(refreshDto.RefreshTokenID, user);

        // 4. Verify Refresh Token expiration or status if needed
        if (!rt.IsActive)
        {
            return Unauthorized("Refresh token has expired.");
        }

        // 5. Generate a new JWT token
        var newAccessToken = await _authHelper.GenerateJwtToken(user);

        // Return the new tokens
        return Ok(new
        {
            AccessToken = newAccessToken,
        });
    }
}
