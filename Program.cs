using BookingSystem.Data;
using BookingSystem.Repositories;
using BookingSystem.Repositories.Interfaces;
using BookingSystem.Services;
using BookingSystem.Services.Interfaces;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

// Load .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Get connection string from .env
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

// Add DbContext
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add Controllers
builder.Services.AddControllers();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});

// Add Repository and Service
builder.Services.AddScoped<IResourceRepository, ResourceRepository>();
builder.Services.AddScoped<IResourceService, ResourceService>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IReservationService, ReservationService>();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.MapControllers();
app.Run();