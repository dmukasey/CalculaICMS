using ApplicationServices.NfeService;
using ICMS.MongoDBHelper;
using ICMS.RabbitMQ;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

IConfiguration configuration;
configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                //.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true) // <- This gives you access to your application settings in your local development environment
                .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                .AddEnvironmentVariables() // <- This is what actually gets you the application settings in Azure
                .Build();

var host = new HostBuilder()
.ConfigureFunctionsWorkerDefaults()
.ConfigureServices(services =>
{
    services.AddApplicationInsightsTelemetryWorkerService();
    services.ConfigureFunctionsApplicationInsights();

    services.Configure<MongoDBSettings>(configuration.GetSection("MongoDBSettings"));
    services.AddSingleton(typeof(MongoDBService));

    services.AddSingleton<IRabbitMensagemRepository, RabbitMensagemRepository>();
    services.AddScoped<INfeService, NfeService>();
})
.Build();

host.Run();
