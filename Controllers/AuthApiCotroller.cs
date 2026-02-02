using Jamghat.Models.Auth;
using Microsoft.AspNetCore.Mvc;
using static Jamghat.Models.Auth.models;

namespace Jamghat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly IAuth _authService;

        public AuthApiController(IAuth authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            if (login == null || string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.Password))
                return BadRequest(new { message = "Username and password are required." });

            var result = _authService.GetUser(login.Username, login.Password);

            if (!result.success)
                return BadRequest(new { message = result.message });

            return Ok(new
            {
                token = result.token,
               
            });
        }


    }


}
