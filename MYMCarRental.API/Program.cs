using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MYMCarRental.API.Middleware;
using MYMCarRental.Application.Interfaces;
using MYMCarRental.Application.Settings;
using MYMCarRental.Infrastructure.Data;
using MYMCarRental.Infrastructure.Email;
using MYMCarRental.Infrastructure.Services;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


// ========================================================
// Database
// ========================================================

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")));


// ========================================================
// Cloudinary
// ========================================================

var cloudinarySettings =
    builder.Configuration.GetSection("Cloudinary");

var cloudName = cloudinarySettings["CloudName"];
var apiKey = cloudinarySettings["ApiKey"];
var apiSecret = cloudinarySettings["ApiSecret"];

var account = new Account(
    cloudName,
    apiKey,
    apiSecret);

var cloudinary = new Cloudinary(account);

builder.Services.AddSingleton(cloudinary);


// ========================================================
// Application Services
// ========================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "http://localhost:4000",
                "https://mym-car-rental.netlify.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddScoped<IImageStorageService,CloudinaryImageStorageService>();

builder.Services.AddScoped<ICategoryService,CategoryService>();

builder.Services.AddScoped<ICarService,CarService>();

builder.Services.AddScoped<IUserService,UserService>();

builder.Services.AddScoped<IAuthService,AuthService>();

builder.Services.AddScoped<IJwtService,JwtService>();

builder.Services.AddScoped<IBookingService,BookingService>();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection(
        EmailSettings.SectionName));

builder.Services.AddScoped<IContactService, ContactService>();




// ========================================================
// Rate Limiter
// ========================================================

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode =
        StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.ContentType =
            "application/json";

        await context.HttpContext.Response
            .WriteAsJsonAsync(
                new
                {
                    success = false,
                    message =
                        "Too many requests. Please try again later."
                },
                cancellationToken: token);
    };


    // ====================================================
    // Authentication Endpoints
    // 5 requests / minute
    // ====================================================

    options.AddPolicy("AuthLimiter", httpContext =>
    {
        var key =
            httpContext.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: key,

            factory: _ =>
                new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,

                    Window = TimeSpan.FromMinutes(1),

                    QueueLimit = 0,

                    AutoReplenishment = true
                });
    });


    // ====================================================
    // Refresh Token
    // 10 requests / minute
    // ====================================================

    options.AddPolicy("RefreshLimiter", httpContext =>
    {
        var key =
            httpContext.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: key,

            factory: _ =>
                new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,

                    Window = TimeSpan.FromMinutes(1),

                    QueueLimit = 0,

                    AutoReplenishment = true
                });
    });


    // ====================================================
    // General Authenticated Endpoints
    // 60 requests / minute
    // ====================================================

    options.AddPolicy("GeneralLimiter", httpContext =>
    {
        var userId =
            httpContext.User
                .FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;

        var key =
            userId ??
            httpContext.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: key,

            factory: _ =>
                new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 60,

                    Window = TimeSpan.FromMinutes(1),

                    QueueLimit = 0,

                    AutoReplenishment = true
                });
    });

    // ====================================================
    // Contact Form
    // 3 requests / 10 minutes
    // ====================================================

    options.AddPolicy("ContactLimiter", httpContext =>
    {
        var key =
            httpContext.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: key,

            factory: _ =>
                new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 3,

                    Window = TimeSpan.FromMinutes(10),

                    QueueLimit = 0,

                    AutoReplenishment = true
                });
    });
});

// ========================================================
// JWT Settings
// ========================================================

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

var jwtSettings =
    builder.Configuration
        .GetSection("Jwt")
        .Get<JwtSettings>()
    ?? throw new InvalidOperationException(
        "JWT settings are not configured.");

if (string.IsNullOrWhiteSpace(jwtSettings.Key))
{
    throw new InvalidOperationException(
        "JWT Key is not configured.");
}


// ========================================================
// Authentication
// ========================================================

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key)),

                ClockSkew = TimeSpan.FromSeconds(30)
            };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken =
                    context.Request.Cookies["mym_access_token"];

                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });


// ========================================================
// Authorization
// ========================================================

builder.Services.AddAuthorization();


// ========================================================
// Swagger
// ========================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "MYM Car Rental API",
            Version = "v1"
        });

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",

            Type = SecuritySchemeType.Http,

            Scheme = "bearer",

            BearerFormat = "JWT",

            In = ParameterLocation.Header,

            Description =
                "Enter your JWT token."
        });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [
                new OpenApiSecuritySchemeReference(
                    "Bearer",
                    document)
            ] = []
        });
});

var app = builder.Build();


// ========================================================
// Middleware
// ========================================================

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();

app.UseSwaggerUI();

app.UseCors("AngularClient");

app.UseAuthentication();

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();