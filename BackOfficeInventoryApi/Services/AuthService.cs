using BackOfficeInventoryApi.Data;
using BackOfficeInventoryApi.Models;
using BackOfficeInventoryApi.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace BackOfficeInventoryApi.Services
{
    public class AuthService : IAuthService
    {

        private readonly ToDoContext _dbContext;
        private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _configuration;

        public AuthService(ToDoContext dbContext,ILogger<AuthService> logger,IConfiguration configuration)
        {
            _dbContext = dbContext;
            _logger =  logger;
            _configuration = configuration;
        }

        public async Task<ActionResult<User?>> RegisterAsync(UserDto request)
        {
            if(await _dbContext.Users.AnyAsync(u=>u.Email==request.Email)) {                
                return new ConflictObjectResult(new { Message = "Userready exists." });
            }
                 
            var user = new User();
            var hashedPassword = new PasswordHasher<User>().HashPassword(user,request.PasswordHashed); 

            user.Email = request.Email;
            user.PasswordHashed = hashedPassword;

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            return user;
        }


        public async Task<ActionResult> LogInAsync(UserDto request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

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

            var token =  CreateToken(user);
            return new OkObjectResult(new { Token = token });
        }


        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Role,user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:SecretKey")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer : _configuration.GetValue<string>("AppSettings: Issuer"),
                audience:_configuration.GetValue<string>("AppSettings: Audience"),
                claims:claims,
                expires:DateTime.UtcNow.AddDays(1),
                signingCredentials:creds

                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }






        public Task<ActionResult> RefreshTokenAsync(User request)
        {
            throw new NotImplementedException();
        }

   
    }
}
