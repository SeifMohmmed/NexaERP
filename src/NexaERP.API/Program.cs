using NexaERP.API;
using NexaERP.API.Extensions;
using NexaERP.DAL;
using NexaERP.DAL.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddApiServices()
        .AddObservability();

builder.Services
       .AddSwaggerDocumentation()
       .AddDatabase(builder.Configuration)
       .AddInfrastructure();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await app.Services.ApplyMigrationsAsync();

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync().ConfigureAwait(false);
