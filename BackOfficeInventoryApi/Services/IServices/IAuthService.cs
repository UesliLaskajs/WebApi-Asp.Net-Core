using BackOfficeInventoryApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackOfficeInventoryApi.Services.IServices
{
    public interface IAuthService
    {
        Task<ActionResult<User?>> RegisterAsync(UserDto request);
        Task<ActionResult> LogInAsync(UserDto request);
        Task<ActionResult<ResponseTokens?>> RefreshTokenAsync(RefreshToken request);

    }
}
