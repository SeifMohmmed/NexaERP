using NexaERP.API;
using NexaERP.DAL;
using NexaERP.DAL.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddApiServices();

builder.Services
       .AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await app.ApplyMigrationsAsync();
    await app.SeedInitialDataAsync();
    await app.SeedAdminUserAsync();

}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

await app.RunAsync().ConfigureAwait(false);
