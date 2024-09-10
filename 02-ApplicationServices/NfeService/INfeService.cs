using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServices.NfeService
{
    public interface INfeService
    {
        public void sendRabbitMQMessage(ICMS.RabbitMQ.RabbitMensagem mensagem, string fila);
        public void ProcessarNFE(ICMS.RabbitMQ.RabbitMensagem message);
    }
}
