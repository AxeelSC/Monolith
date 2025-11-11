using Microsoft.AspNetCore.Authentication.JwtBearer;
using HexagonalModular.Infrastructure.Auth;
using HexagonalModular.Infrastructure.Persistence;
using HexagonalModular.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HexagonalModular.Application.Interfaces.User;
using HexagonalModular.Application.Authentication.Interfaces;
using HexagonalModular.Application.Security;
using HexagonalModular.Application;
using HexagonalModular.Infrastructure.User;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

#if DEBUG
    Console.WriteLine($"[DEBUG] JWT Key length: {jwtKey?.Length}");
    Console.WriteLine($"[DEBUG] JWT Issuer: {jwtIssuer}");
    Console.WriteLine($"[DEBUG] JWT Audience: {jwtAudience}");
#endif

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
#if DEBUG
            Console.WriteLine($"[DEBUG] JWT Authentication failed: {context.Exception.Message}");
#endif
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
#if DEBUG
            Console.WriteLine("[DEBUG] JWT Token validated successfully");
#endif
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

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
