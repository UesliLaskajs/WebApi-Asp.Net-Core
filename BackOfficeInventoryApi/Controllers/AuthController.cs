using BackOfficeInventoryApi.Models;
using BackOfficeInventoryApi.Services;
using BackOfficeInventoryApi.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace BackOfficeInventoryApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService=authService;  
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var user=await _authService.RegisterAsync(request);

            if (user is null)
                return BadRequest("Username already exists.");

            return Ok(user);

        }


        [HttpPost("login")]
        public async Task<ActionResult> Login(UserDto request)
        {
            var result = await _authService.LogInAsync(request);
            if (result is null)
                return BadRequest("Invalid username or password.");

            return Ok(result);
        }




        public IActionResult Index()
        {
            throw new NotImplementedException();
        }
    }
}
