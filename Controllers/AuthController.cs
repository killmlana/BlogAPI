using BlogAPI.Helpers;
using BlogAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly NHibernateHelper _nHibernateHelper;
    private readonly AuthHelper _authHelper;

    public AuthController(AuthHelper authHelper, NHibernateHelper nHibernateHelper)
    {
        _nHibernateHelper = nHibernateHelper;
        _authHelper = authHelper;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDTO userDto)
    {
        try
        {
            // Register the user
            var user = _authHelper.CreateUser(userDto);
            
            using (var session = _nHibernateHelper.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                await session.SaveAsync(user);
                await transaction.CommitAsync();
            }

            return Accepted();
        }
        catch (Exception ex)
        {
            // Handle registration errors
            return BadRequest($"Registration failed: {ex.Message}");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDTO userDto)
    {
        
    }
}
