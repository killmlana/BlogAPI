using BlogAPI.Entities;
using BlogAPI.Helpers;
using BlogAPI.Models;
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
            return Ok();
        }
        catch (Exception ex)
        {
            // Handle registration errors
            return BadRequest();
        }
    }

    [HttpPost("login")]
    public async Task<string> Login([FromBody] UserDTO userDto)
    {
        var user = await _authHelper.Login(userDto);
        return await _authHelper.GenerateJwt(user);
    }
}
