using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using DotNetEnv;
using Hangfire;
using BookingSystem.Services.Interfaces;
using BookingSystem.Repositories.Interfaces;
using BookingSystem.Services;
using BookingSystem.Repositories;
using BookingSystem.Data;
using BookingSystem.Jobs;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? builder.Configuration.GetConnectionString("DefaultConnection");
var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY") ?? throw new InvalidOperationException("SECRET_KEY environment variable is not set.");

builder.Services.AddDbContext<BookingDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddControllers();

builder.Services.AddCors(options => options.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddScoped<IResourceRepository, ResourceRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddScoped<IResourceService, ResourceService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<ReservationJob>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        name: JwtBearerDefaults.AuthenticationScheme,
        securityScheme: new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter the Bearer Authorization : 'Bearer Genreated-JWT-Token'",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            }, new string[]{ }
            }
        });
});

builder.Services.AddHangfire((config) => { config.UseSqlServerStorage(connectionString); });
builder.Services.AddHangfireServer();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Environment.GetEnvironmentVariable("ISSUER"),
        ValidAudience = Environment.GetEnvironmentVariable("AUDIENCE"),
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey))
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard();
app.MapControllers();
app.Run();