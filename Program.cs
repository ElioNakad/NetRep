using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyAzureDemo.Data;
using MyAzureDemo.Models;
using MyAzureDemo.Options;
using MyAzureDemo.Services;
using MyAzureDemo.Utilities;
using System.Text;

DotEnvLoader.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = GetPostgresConnectionString(builder.Configuration);
var jwtSettings = GetJwtSettings(builder.Configuration);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

if (app.Configuration.GetValue("Database:RunMigrationsOnStartup", false))
{
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static JwtSettings GetJwtSettings(IConfiguration configuration)
{
    var settings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();

    if (string.IsNullOrWhiteSpace(settings.Key) || settings.Key.Length < 32)
    {
        throw new InvalidOperationException(
            "Missing JWT key. Set Jwt:Key or Jwt__Key to a random value at least 32 characters long.");
    }

    settings.Issuer = FirstConfiguredValue(settings.Issuer, "MyAzureDemo")!;
    settings.Audience = FirstConfiguredValue(settings.Audience, "MyAzureDemo")!;

    if (settings.ExpirationMinutes <= 0)
    {
        settings.ExpirationMinutes = 60;
    }

    return settings;
}


static string GetPostgresConnectionString(IConfiguration configuration)
{
    var connectionString =
        FirstConfiguredValue(
            configuration.GetConnectionString("DefaultConnection"),
            Environment.GetEnvironmentVariable("POSTGRESQLCONNSTR_DefaultConnection"),
            Environment.GetEnvironmentVariable("DATABASE_URL"));

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException(
            "Missing PostgreSQL connection string. Set ConnectionStrings:DefaultConnection, " +
            "ConnectionStrings__DefaultConnection, POSTGRESQLCONNSTR_DefaultConnection, or DATABASE_URL.");
    }

    return connectionString;
}

static string? FirstConfiguredValue(params string?[] values)
{
    return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
}
