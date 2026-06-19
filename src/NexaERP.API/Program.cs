using NexaERP.API;
using NexaERP.DAL;
using NexaERP.DAL.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddApiServices()
        .AddObservability();

builder.Services.AddDatabase(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await app.Services.ApplyMigrationsAsync();

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync().ConfigureAwait(false);
