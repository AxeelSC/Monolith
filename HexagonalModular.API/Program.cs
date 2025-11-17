using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HexagonalModular.Application.Identity.Common.Ports;
using HexagonalModular.Application.Identity.Common.Security;
using HexagonalModular.Application.Identity.Common.Persistence;
using HexagonalModular.Infrastructure.Identity.Persistence;
using HexagonalModular.Infrastructure.Identity.Security;
using HexagonalModular.Infrastructure.Identity.Persistence.Repositories;
using HexagonalModular.API.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Log a consola
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // Log en archivo
    .CreateLogger();

builder.Host.UseSerilog(); // Usar Serilog en lugar del logger predeterminado de .NET

/// <summary>
/// Configura los servicios necesarios para la aplicación,
/// incluyendo la autenticación JWT, la base de datos PostgreSQL, y los repositorios.
/// </summary>
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuración de JWT para autenticar a los usuarios
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtKey = builder.Configuration["Jwt:Key"];
    var jwtIssuer = builder.Configuration["Jwt:Issuer"];
    var jwtAudience = builder.Configuration["Jwt:Audience"];

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            return Task.CompletedTask;
        }
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Configuración de los repositorios
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IIdentityUnitOfWork, IdentityUnitOfWork>();

var app = builder.Build();

// Middleware global de excepciones
app.UseMiddleware<ExceptionHandlingMiddleware>();

/// <summary>
/// Configura el pipeline de peticiones HTTP.
/// Se incluyen los middleware de autorización y los mapeos de controladores.
/// </summary>
app.UseAuthorization();
app.MapControllers();

// Ejecutar la aplicación
app.Run();
