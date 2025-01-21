using Microsoft.EntityFrameworkCore;
using Serilog;
using ChristmasGiftApi.Data;
using ChristmasGiftApi.Services;
using ChristmasGiftApi.Interfaces;
using ChristmasGiftApi.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ChristmasGiftApi.Models;

var builder = WebApplication.CreateBuilder(args);
Console.WriteLine($"Application starting at: {DateTime.UtcNow}");


// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/christmas-gift-api-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Christmas Gift API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});



// Add JWT authentication
builder.Services.AddAuthentication(options =>
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found")))
    };
});

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IBaseService<UserDto, CreateUserDto, UpdateUserDto>, UserService>();
builder.Services.AddScoped<IBaseService<WishListItemDto, CreateWishListItemDto, UpdateWishListItemDto>, WishListService>();
builder.Services.AddScoped<IBaseService<HeroContentDto, CreateHeroContentDto, UpdateHeroContentDto>, HeroContentService>();
builder.Services.AddScoped<IBaseService<WishListSubmissionDto, CreateWishListSubmissionDto, UpdateWishListSubmissionDto>, WishListSubmissionService>();
builder.Services.AddScoped<IBaseService<RecommendWishListItemDto, CreateRecommendWishListItemDto, UpdateRecommendWishListItemDto>, RecommendWishListService>();

builder.Services.Configure<NotificationSettings>(
    builder.Configuration.GetSection("NotificationSettings"));
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped<WishListNotificationService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();

builder.Services.AddMemoryCache();

var app = builder.Build();
//Console.WriteLine($"Configuration values loaded:");
//Console.WriteLine($"ASPNETCORE_URLS: {Environment.GetEnvironmentVariable("ASPNETCORE_URLS")}");
//Console.WriteLine($"WEBSITES_PORT: {Environment.GetEnvironmentVariable("WEBSITES_PORT")}");
//app.Urls.Add("http://*:8080");  // Make sure this is added

Console.WriteLine("Application built and configured, starting...");

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

// Use CORS before authorization
app.UseCors("AllowAll");

// Add authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    Console.WriteLine($"Let's run it");
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Console.WriteLine("FLUSHING LOGS");
    Log.CloseAndFlush();
}