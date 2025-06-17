using Mnf_Portal.APIs.Extensions;

var builder = WebApplication.CreateBuilder(args);

#region Configure Services
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddCorsPolicies();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

#endregion
var app = builder.Build();

await app.ApplyMigrationsAndSeedDataAsync();
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
