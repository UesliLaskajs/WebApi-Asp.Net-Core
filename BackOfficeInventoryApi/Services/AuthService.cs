using BackOfficeInventoryApi.Data;
using BackOfficeInventoryApi.Models;
using BackOfficeInventoryApi.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class AuthService : IAuthService
{
    private readonly ToDoContext _dbContext;
    private readonly ILogger<AuthService> _logger;
    private readonly IConfiguration _configuration;

    public AuthService(ToDoContext dbContext, ILogger<AuthService> logger, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<ActionResult<User?>> RegisterAsync(UserDto request)
    {
        if ( await _dbContext.Users.AnyAsync(u => u.Email == request.Email))
        {
            return new ConflictObjectResult(new { Message = "User already exists." });
        }

        var user = new User();
        var hashedPassword = new PasswordHasher<User>().HashPassword(user, request.PasswordHashed);
        user.Email = request.Email;
        user.PasswordHashed = hashedPassword;

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<ActionResult> LogInAsync(UserDto request)
    {
        var user =  _dbContext.Users.FirstOrDefault(u => u.Email == request.Email);

        if (user == null)
        {
            return new UnauthorizedObjectResult(new { Message = "Invalid email or password." });
        }

        var passwordHasher = new PasswordHasher<User>();
        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHashed, request.PasswordHashed);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            return new UnauthorizedObjectResult(new { Message = "Invalid email or password." });
        }

        var token = CreateToken(user);
        return new OkObjectResult(new { Token = token });
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:SecretKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _configuration["AppSettings:Issuer"],
            audience: _configuration["AppSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    public async Task<ActionResult<ResponseTokens>> CreateTokenResponse(User request)
    {
        var responseTokens = new ResponseTokens
        {
            AccessToken = CreateToken(request),
            RefreshTokens = await GenerateAndSaveAsync(request)
        };

        return new OkObjectResult(responseTokens);
    }

    public async Task<ActionResult<ResponseTokens?>> RefreshTokenAsync(RefreshToken request)
    {
        var user = await ValidateRefreshToken(request.UserId, request.RefreshTokens);

        _logger.LogInformation("Inside RefreshTokenAsync", user);
        if (user == null)
        {
            return new UnauthorizedObjectResult(new { Message = "Invalid refresh token." });
        }

        return await CreateTokenResponse(user);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private async Task<string> GenerateAndSaveAsync(User user)
    {
        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Refresh token expiry (7 days)
        _logger.LogInformation($"User state before save: {user}");
        await _dbContext.SaveChangesAsync();
        return refreshToken;
    }

    private async Task<User?> ValidateRefreshToken(Guid userId, string refreshToken)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null;
        }

        return user;
    }
}
