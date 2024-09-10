using ApplicationServices.NfeService;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;


namespace NFE.Controllers
{
    [ApiController]   
    [Route("[controller]")]
    public class NFEController : ControllerBase
    {
        private INfeService _nfeService;
        public IConfiguration Configuration { get; }

        public NFEController(IConfiguration configuration, INfeService nfeService)
        {
            _nfeService = nfeService;    
            Configuration = configuration;
        }       
        

      

        [HttpPost("UploadNFE")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Nenhum arquivo foi enviado.");

            if (! file.FileName.Contains(".xml") )
                return BadRequest("O arquivo enviado não é um XML");

            string filePath = Configuration["filePath"] + "/"+ file.FileName;

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }


                var guid = Guid.NewGuid().ToString();
                ICMS.RabbitMQ.RabbitMensagem mensagem = new ICMS.RabbitMQ.RabbitMensagem();
                mensagem.Titulo = guid;
                mensagem.Texto = filePath;                

                _nfeService.sendRabbitMQMessage(mensagem, "fila-nfe-processar");

                return Ok(new { message = "Arquivo enviado com sucesso!", filePath = filePath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
    }
}
