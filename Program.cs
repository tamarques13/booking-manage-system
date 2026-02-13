using BookingSystem.Data;
using BookingSystem.Repositories;
using BookingSystem.Repositories.Interfaces;
using BookingSystem.Services;
using BookingSystem.Services.Interfaces;
using BookingSystem.Jobs;
using DotNetEnv;
using Hangfire;
using Microsoft.EntityFrameworkCore;

Env.Load();

var builder = WebApplication.CreateBuilder(args);
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<BookingDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddControllers();

builder.Services.AddCors(options => options.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddScoped<IResourceRepository, ResourceRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IResourceService, ResourceService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<ReservationJob>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHangfire((config) =>
{
    config.UseSqlServerStorage(connectionString);
});
builder.Services.AddHangfireServer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.MapControllers();
app.UseHangfireDashboard();
app.Run();