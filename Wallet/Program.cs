using Serilog;
using Wallet.Application;
using Wallet.Infrastructure;
using Wallet.ServiceExtenstions;

var configuration = new ConfigurationBuilder()
                                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                            .Build();
var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
WebApplication app;

try
{
    // Add services to the container.-
    builder.Services.AddControllers();

    var services = builder.Services;
    services.AddApplicationServices();
    services.AddInfrastructureServices(builder.Configuration);
    services.AddJWtService(builder.Configuration);
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerWithAuthorization();

    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    builder.Host.UseSerilog(logger);
    app = builder.Build();

    logger.Information($"Wallet API Starting Up! in {environment} Environment");
}
catch (Exception ex)
{
    logger.Fatal(ex, "The Wallet API Failed to Start Correctly.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
    app.UseHttpsRedirection();


app.UseRequestLocalization();

//app.UseApiWrapper();

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
