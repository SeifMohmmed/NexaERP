using NexaERP.API;

var builder = WebApplication.CreateBuilder(args);

builder.AddApiServices()
        .AddObservability();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync().ConfigureAwait(false);
