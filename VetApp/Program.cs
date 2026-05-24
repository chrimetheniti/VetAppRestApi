
using Microsoft.EntityFrameworkCore;
using VetApp.Data;
using VetApp.Repositories;

namespace VetApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connString= builder.Configuration.GetConnectionString("DevConnection");

            builder.Services.AddDbContext<VetAppDbContext>(options =>
                    options.UseSqlServer(connString));
            // Add services to the container.

            builder.Services.AddRepositories();

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            //builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
