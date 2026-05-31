
using Microsoft.EntityFrameworkCore;
using VetApp.Configuration;
using VetApp.Data;
using VetApp.Repositories;
using VetApp.Security;
using VetApp.Services;



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
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IVeterinarianService, VeterinarianService>();
            builder.Services.AddScoped<IPatientService, PatientService>();
            builder.Services.AddScoped<IOwnerService, OwnerService>();
            builder.Services.AddScoped<IApplicationService, ApplicationService>();
            builder.Services.AddSingleton<IEncryptionUtil, EncryptionUtil>();
            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapperConfig>());
            

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
