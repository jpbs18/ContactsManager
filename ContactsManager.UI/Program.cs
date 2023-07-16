using Serilog;
using CrudExample.StartupExtensions;
using CrudExample.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration configuration) =>
{
    configuration
    .ReadFrom.Configuration(context.Configuration) // read configurations from built in IConfiguration (appsettings.json)
    .ReadFrom.Services(services); // read out services and make them available to Serilog
});

// built in logging framework
/*builder.Host.ConfigureLogging(loggingProvider =>
{
    loggingProvider.ClearProviders();
    loggingProvider.AddConsole();
    loggingProvider.AddDebug();
});*/

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseExceptionHandleMiddleware();
}

Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

app.UseStaticFiles();
app.UseRouting();
app.UseHttpLogging();
app.MapControllers();
app.Run();
