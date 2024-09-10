using System;
//using ICMS.RabbitMQ;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;


namespace WorkerNFE
{
    public class nfeFunction
    {
        [FunctionName("nfeFunction")]
        public void Run([RabbitMQTrigger("fila-nfe-processar", ConnectionStringSetting = "rabbit_cnstring")]string message, ILogger log)
        {
            Processar(message);
        }

        private void Processar(string message)
        {
            Console.WriteLine("Mensagem recebida: "+ message);

            //RabbitMensagem mensagem = Newtonsoft.Json.JsonConvert.DeserializeObject<RabbitMensagem>(message);
        }
    }
}
