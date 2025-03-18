using BackOfficeInventoryApi.Data;
using BackOfficeInventoryApi.GlobalExecptionHandler;
using BackOfficeInventoryApi.Repository;
using BackOfficeInventoryApi.Repository.IRepository;
using BackOfficeInventoryApi.Services;
using BackOfficeInventoryApi.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
[assembly: ApiController]
namespace BackOfficeInventoryApi

{
    public class Program
    {

        public static  void Main(string[] args)
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


                //builder.Services.AddDbContext<ToDoContext>(opt => opt.UseInMemoryDatabase("ToDoList"));
                builder.Services.AddDbContext<ToDoContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                builder.Services.AddScoped<IProductRepository, ProductRepository>();
                builder.Services.AddScoped<IProductServices, ProductServices>();

                var app = builder.Build();

                app.UseMiddleware<CustomExceptionHandler>();



                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    //app.UseExceptionHandler("/error");
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseAuthorization();


                app.MapControllers();

                app.Run();
            }
            catch(Exception ex)
            {
                Log.Error("Something Went Wrong", ex.Message);
            }
            finally
            {
                 Log.CloseAndFlushAsync();
            }
        }
    }
}
