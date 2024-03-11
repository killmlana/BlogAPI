using System.Text.RegularExpressions;
using BlogAPI.Helpers;
using BlogAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AuthController(Registration registration) : ControllerBase
{

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDTO userDto)
    {
        try
        {
            //TODO Validate userDto
            if (Regex.IsMatch(userDto.username, "[^0-9a-zA-Z]+"))
            {
                return BadRequest($"Registration failed: Only alphanumeric characters are allowed in username.");
            }
            
            
            
            // Register the user
            var user = registration.CreateUser(userDto.username, userDto.password);
            
            using (var session = NHibernateHelper.OpenSession())
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
}
