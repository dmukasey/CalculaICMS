using ICMS.RabbitMQ;
using ApplicationServices;
using ApplicationServices.NfeService;
using ICMS.MongoDBHelper;
using Microsoft.Extensions.Configuration;
using System.Reflection;

IConfiguration configuration;
configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                //.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true) // <- This gives you access to your application settings in your local development environment
                .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                .AddEnvironmentVariables() // <- This is what actually gets you the application settings in Azure
                .Build();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IRabbitMensagemRepository, RabbitMensagemRepository>();
builder.Services.AddScoped<INfeService, NfeService>();

builder.Services.Configure<MongoDBSettings>(configuration.GetSection("MongoDBSettings"));
builder.Services.AddSingleton(typeof(MongoDBService));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
