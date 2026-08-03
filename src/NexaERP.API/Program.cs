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
       .AddInfrastructure()
       .AddAuthenticationService(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await app.ApplyMigrationsAsync();

}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync().ConfigureAwait(false);
