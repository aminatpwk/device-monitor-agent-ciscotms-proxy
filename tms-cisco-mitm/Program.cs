using tms_cisco_mitm.Services;
using tms_cisco_mitm.Services.Interface;
using tms_cisco_mitm.Worker;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Cisco TMS Proxy API",
        Version = "v1",
        Description = "SOAP/XML gateway that impersonates Cisco TMS server"
    });
});
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Services.AddSingleton<IRawMessageStore, FileBasedRawMessageStore>();
builder.Services.AddSingleton<InMemoryMessageQueue>();
builder.Services.AddSingleton<IMessageQueuePublisher>(sp => sp.GetRequiredService<InMemoryMessageQueue>());
builder.Services.AddSingleton<ISoapParserService, SoapParserService>();
builder.Services.AddHttpClient<ITelemetryForwarder, TelemetryForwarder>()
    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30);
    });
builder.Services.AddHostedService<MessageProcessorWorker>();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Cisco TMS Proxy API v1");
        options.RoutePrefix = string.Empty; 
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Cisco TMS Proxy starting...");
logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
logger.LogInformation("Raw message storage directory: {Directory}", builder.Configuration["RawMessageStore:Directory"] ?? "raw-messages");

app.Run();
