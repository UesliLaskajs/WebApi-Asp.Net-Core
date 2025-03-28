using BackOfficeInventoryApi.Data;
using BackOfficeInventoryApi.GlobalExecptionHandler;
using BackOfficeInventoryApi.Repository;
using BackOfficeInventoryApi.Repository.IRepository;
using BackOfficeInventoryApi.Services;
using BackOfficeInventoryApi.Services.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;

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
                    options.AddPolicy("AllowSpecificOrigins", policy =>
                    {
                        policy.WithOrigins("http://localhost:5007")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
                });

                builder.Services.AddRateLimiter(options =>
                {

                    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                          RateLimitPartition.GetFixedWindowLimiter(
                                partitionKey: context.Connection.RemoteIpAddress?.ToString(), 
                                factory: _ => new FixedWindowRateLimiterOptions
                                {
                                    PermitLimit = 5,
                                    Window = TimeSpan.FromSeconds(10),
                                    QueueLimit = 2
                                }));

                    options.AddFixedWindowLimiter("FixedPolicy", limiterOptions =>
                    {
                        limiterOptions.PermitLimit = 5;
                        limiterOptions.Window = TimeSpan.FromSeconds(10);
                        limiterOptions.QueueLimit = 2;
                    });

                    options.AddTokenBucketLimiter("TokenBucketPolicy", limiterOptions =>
                    {
                        limiterOptions.TokenLimit = 10;     // Max tokens
                        limiterOptions.TokensPerPeriod = 2; // Tokens refilled per period
                        limiterOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(5);
                        limiterOptions.QueueLimit = 2;
                    });

                    options.AddSlidingWindowLimiter("SlidingPolicy", limiterOptions =>
                    {
                        limiterOptions.PermitLimit = 5;
                        limiterOptions.Window = TimeSpan.FromSeconds(30);
                        limiterOptions.SegmentsPerWindow = 3;
                        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                        limiterOptions.QueueLimit = 2;
                    });

                    options.AddConcurrencyLimiter("ConcurrencyPolicy", limiterOptions =>
                    {
                        limiterOptions.PermitLimit = 3; // Max 3 concurrent requests
                        limiterOptions.QueueLimit = 2;
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
                app.UseCors("AllowSpecificOrigins"); // Apply CORS
                app.UseRateLimiter();

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
