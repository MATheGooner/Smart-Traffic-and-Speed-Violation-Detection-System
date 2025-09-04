using Microsoft.EntityFrameworkCore;
using TrafficControlSystem;
using TrafficControlSystem.Context;
using TrafficControlSystem.Entities;
using TrafficControlSystem.Implementation.Respositories;
using TrafficControlSystem.Implementation.Services;
using TrafficControlSystem.Interface.Respositories;
using TrafficControlSystem.Interface.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(x => x.AddPolicy("Policies", c =>
{
    c.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
}));
// Add services to the container.
builder.Services.AddScoped<IImageRepo, ImageRepo>();
builder.Services.AddScoped<ILaneRepo, LaneRepo>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IViolationRepo, ViolationRepo>();
builder.Services.AddScoped<ITrafficDensityRepo, TrafficDensityRepo>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<ILaneService, LaneService>();
builder.Services.AddScoped<IViolationService, ViolationService>();
builder.Services.AddScoped<ITrafficDensityService, TrafficDensityService>();
builder.Services.AddHostedService<TBackgroundService>();
builder.Services.AddHttpContextAccessor();
var connectionString = builder.Configuration.GetConnectionString("TrafficControlSystemContext");
connectionString = $"Server={Environment.GetEnvironmentVariable("MYSQLHOST")};" +
                       $"Port={Environment.GetEnvironmentVariable("MYSQLPORT")};" +
                       $"Database={Environment.GetEnvironmentVariable("MYSQLDATABASE")};" +
                       $"User={Environment.GetEnvironmentVariable("MYSQLUSER")};" +
                       $"Password={Environment.GetEnvironmentVariable("MYSQLPASSWORD")};";
Console.WriteLine($"Connection String: {connectionString}");
builder.Services.AddDbContext<TrafficControlSystemContext>(c => c.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Traffic Control System", Version = "v1" });
});

var app = builder.Build();


app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("Policies");

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
