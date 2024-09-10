
namespace ICMS.RabbitMQ
{
    public interface IRabbitMensagemRepository
    {
        void SendMensagem(RabbitMensagem mensagem, string fila);
    }


}
