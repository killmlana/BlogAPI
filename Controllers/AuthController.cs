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

    public AuthController(AuthHelper authHelper, NHibernateHelper nHibernateHelper, UserManager<User> manager)
    {
        _nHibernateHelper = nHibernateHelper;
        _authHelper = authHelper;
        _manager = manager;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDTO userDto)
    {
        try
        {
            // Register the user
            var user = _authHelper.CreateUser(userDto);
            var result = await _manager.CreateAsync(user);
            return Accepted();
        }
        catch (Exception ex)
        {
            // Handle registration errors
            return BadRequest($"Registration failed: {ex.Message}");
        }
    }

    /*[HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDTO userDto)
    {
        await 
    }*/
}
