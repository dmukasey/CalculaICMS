using System;
using ApplicationServices.NfeService;
using Google.Protobuf;
using ICMS.RabbitMQ;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ICMS.Workers.NFE
{
    

    public class Function1
    {
        private INfeService _service;
        private readonly ILogger _logger;

        public Function1(ILoggerFactory loggerFactory, INfeService service)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
            _service = service;
        }

        [Function("nfeFunction")]
        public void Run([RabbitMQTrigger("fila-nfe-processar", ConnectionStringSetting = "rabbit_cnstring")] string mensagem)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {mensagem}");

            Processar(mensagem);
        }

      
        private void Processar(string mensagem)
        {
            Console.WriteLine("Mensagem recebida: " + mensagem);

            RabbitMensagem message = Newtonsoft.Json.JsonConvert.DeserializeObject<RabbitMensagem>(mensagem);

           _service.ProcessarNFE(message);

        }
    }
}
