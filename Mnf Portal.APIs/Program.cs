using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mnf_Portal.APIs.Errors;
using Mnf_Portal.APIs.Helpers;
using Mnf_Portal.Core.Entities.Identity;
using Mnf_Portal.Core.Interfaces;
using Mnf_Portal.Core.Services;
using Mnf_Portal.Infrastructure.Identity;
using Mnf_Portal.Infrastructure.Identity.IdentityDataSeed;
using Mnf_Portal.Infrastructure.Persistence;
using Mnf_Portal.Infrastructure.Persistence.Repositories;
using Mnf_Portal.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
#region Configure Services

// Add services to the container.
builder.Services.AddScoped<EmailService>();

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MnfDbContext>(options => options.UseSqlServer(connectionString));

builder.Services
    .AddDbContext<MnfIdentityDbContext>(
        options => options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

builder.Services.AddScoped(typeof(IMnfContextRepo<>), typeof(MnfContextRepo<>));
builder.Services.AddScoped(typeof(IMnfIdentityContextRepo<>), typeof(MnfIdentityContextRepo<>));
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<MnfIdentityDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = true; //@, #, $, %, !, ?, and other special characters
});

builder.Services
    .AddAuthentication(
        options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
    .AddJwtBearer(
        options =>
        {
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["accessToken"];
                    return Task.CompletedTask;
                }
            };

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JWT:Issuer"],
                ValidAudience = builder.Configuration["JWT:Audience"],
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]!))
            };
        });

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "FrontendPolicy",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services
    .Configure<ApiBehaviorOptions>(
        option => option.InvalidModelStateResponseFactory =
            actionContext =>
            {
                var errors = actionContext.ModelState
                    .Where(P => P.Value!.Errors.Count > 0)
                    .SelectMany(P => P.Value!.Errors)
                    .Select(E => E.ErrorMessage)
                    .ToArray();
                var responseError = new ApiValidationErrorResponse { Errors = errors };

                return new BadRequestObjectResult(responseError);
            });
#endregion
var app = builder.Build();

using var scope = app.Services.CreateScope(); // Creates a new dependency injection scope.

var services = scope.ServiceProvider;

var loggerFactory = services.GetRequiredService<ILoggerFactory>();

try
{
    var dbContext = services.GetRequiredService<MnfDbContext>();
    await dbContext.Database.MigrateAsync();  // Apply migrations at startup

    await MnfDbContextSeed.SeedingAsync(dbContext);

    var identityContext = services.GetRequiredService<MnfIdentityDbContext>(); // Ask CLR For Creating Object From AppIdentityDbContext Explicitly

    await identityContext.Database.MigrateAsync();

    var userManager = services.GetRequiredService<UserManager<AppUser>>();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await AppIdentityDbContextSeed.SeedUsersAsync(userManager, roleManager);
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An Error Occurs When Applying The Migrations");
}
#region Kestrel Middlewares

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();
#endregion
app.Run();
