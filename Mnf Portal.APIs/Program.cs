using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
//.AddJsonOptions(options =>
//{
//    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
//    options.JsonSerializerOptions.WriteIndented = true;
//})
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MnfDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDbContext<MnfIdentityDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<MnfIdentityDbContext>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
                .AddJwtBearer(
                    options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = builder.Configuration["JWT:Issuer"],
                            ValidAudience = builder.Configuration["JWT:Audience"],
                            IssuerSigningKey =
                                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]!))
                        };
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

    await AppIdentityDbContextSeed.SeedUsersAsync(userManager);
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

app.UseStaticFiles();

app.MapControllers();
#endregion
app.Run();
