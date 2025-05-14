using Microsoft.EntityFrameworkCore;
using Mnf_Portal.APIs.Helpers;
using Mnf_Portal.Core.Interfaces;
using Mnf_Portal.Infrastructure.Persistence;
using Mnf_Portal.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);
#region Configure Services

// Add services to the container.

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

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddAutoMapper(typeof(MappingProfile));
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
