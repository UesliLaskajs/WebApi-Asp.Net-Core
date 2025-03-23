using BackOfficeInventoryApi.Data;
using BackOfficeInventoryApi.GlobalExecptionHandler;
using BackOfficeInventoryApi.Repository;
using BackOfficeInventoryApi.Repository.IRepository;
using BackOfficeInventoryApi.Services;
using BackOfficeInventoryApi.Services.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

[assembly: ApiController]

namespace BackOfficeInventoryApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .WriteTo.Console()
               .CreateLogger();

            Log.Information("Serilog Started");
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog();

                builder.Services.AddControllers()
                    .ConfigureApiBehaviorOptions(options =>
                    {
                        var builtInFactory = options.InvalidModelStateResponseFactory;
                        options.InvalidModelStateResponseFactory = context =>
                        {
                            var logger = context.HttpContext.RequestServices
                                                .GetRequiredService<ILogger<Program>>();
                            return builtInFactory(context);
                        };
                    });

                // Database Connection
                builder.Services.AddDbContext<ToDoContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

                // Authentication Setup
                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["AppSettings:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["AppSettings:Audience"],
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:SecretKey"]!)),
                        ValidateIssuerSigningKey = true
                    };
                });

                // CORS Configuration
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll", policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
                });

                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                // Dependency Injection
                builder.Services.AddScoped<IAuthService, AuthService>();
                builder.Services.AddScoped<IProductRepository, ProductRepository>();
                builder.Services.AddScoped<IProductServices, ProductServices>();

                var app = builder.Build();

                // Middleware Order Fix
                app.UseMiddleware<CustomExceptionHandler>();
                app.UseCors("AllowAll"); // Apply CORS
                app.UseAuthentication();
                app.UseAuthorization();

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();
                app.MapControllers();
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Something Went Wrong");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
