using CoffeeBlog_BackEnd.Models;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var dbConnection = Environment.GetEnvironmentVariable("DB_CONNECTION")
                   ?? throw new InvalidOperationException("DB_CONNECTION is missing in .env");

// Add services to the container.

builder.Services.AddControllers();

// Allow Postman, Swagger, and localhost
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Dependency injection of AppDbContext via connection string w/ sqlserver
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(dbConnection));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
