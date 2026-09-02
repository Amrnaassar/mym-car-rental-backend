using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using MYMCarRental.Application.Interfaces;
using MYMCarRental.Infrastructure.Data;
using MYMCarRental.Infrastructure.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

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

builder.Services.AddScoped<
    IImageStorageService,
    CloudinaryImageStorageService>();
builder.Services.AddScoped<
    ICategoryService,
    CategoryService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();