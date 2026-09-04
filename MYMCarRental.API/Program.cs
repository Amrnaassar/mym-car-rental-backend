using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MYMCarRental.Application.Interfaces;
using MYMCarRental.Application.Settings;
using MYMCarRental.Infrastructure.Data;
using MYMCarRental.Infrastructure.Services;
using System.Text;

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
            .WithOrigins("http://localhost:4200")
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

// ========================================================
// JWT Settings
// ========================================================

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

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

app.UseSwagger();

app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseCors("AngularClient");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();