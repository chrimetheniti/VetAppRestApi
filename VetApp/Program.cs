using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using VetApp.Configuration;
using VetApp.Data;
using VetApp.Helpers;
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
            builder.Host.UseSerilog((hostingContext, configuration) =>
            {
                configuration.ReadFrom.Configuration(hostingContext.Configuration);
            });
            var connString = builder.Configuration.GetConnectionString("DevConnection");

            builder.Services.AddDbContext<VetAppDbContext>(options =>
                options.UseSqlServer(connString));

            builder.Services.AddRepositories();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IVeterinarianService, VeterinarianService>();
            builder.Services.AddScoped<IPatientService, PatientService>();
            builder.Services.AddScoped<IOwnerService, OwnerService>();
            builder.Services.AddScoped<IApplicationService, ApplicationService>();
            builder.Services.AddSingleton<IEncryptionUtil, EncryptionUtil>();
            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapperConfig>());

            // ============================================
            // CORS
            // ============================================
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowClient", policy =>
                    policy.WithOrigins(builder.Configuration["Cors:Origin"]!)
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            // ============================================
            // JWT Authentication
            // ============================================
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                //options.IncludeErrorDetails = builder.Environment.IsDevelopment();
                // options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],

                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,

                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!))
                };
            });

            // ============================================
            // Authorization Policies
            // ============================================
            builder.Services.AddAuthorization(options =>
            {
                // Users
                options.AddPolicy("VIEW_USERS",
                    p => p.RequireClaim("capability", "VIEW_USERS"));

                // Veterinarians
                options.AddPolicy("VIEW_VETERINARIANS",
                    p => p.RequireClaim("capability", "VIEW_VETERINARIANS"));
                options.AddPolicy("DELETE_VETERINARIAN",
                    p => p.RequireClaim("capability", "DELETE_VETERINARIAN"));

                // Patients
                options.AddPolicy("VIEW_PATIENTS",
                    p => p.RequireClaim("capability", "VIEW_PATIENTS"));
                options.AddPolicy("INSERT_PATIENT",
                    p => p.RequireClaim("capability", "INSERT_PATIENT"));
                options.AddPolicy("EDIT_PATIENT",
                    p => p.RequireClaim("capability", "EDIT_PATIENT"));
                options.AddPolicy("DELETE_PATIENT",
                    p => p.RequireClaim("capability", "DELETE_PATIENT"));

                // Owners
                options.AddPolicy("VIEW_OWNERS",
                    p => p.RequireClaim("capability", "VIEW_OWNERS"));
                options.AddPolicy("DELETE_OWNER",
                    p => p.RequireClaim("capability", "DELETE_OWNER"));
            });

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            builder.Services.AddEndpointsApiExplorer();

            // ============================================
            // Swagger / OpenAPI
            // ============================================
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Vet App", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
                    new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme.",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        BearerFormat = "JWT"
                    });

                options.OperationFilter<AuthorizeOperationFilter>();
            });

            // ============================================
            // Exception Handler
            // ============================================
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            var app = builder.Build();

            app.UseSerilogRequestLogging();
            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowClient");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}