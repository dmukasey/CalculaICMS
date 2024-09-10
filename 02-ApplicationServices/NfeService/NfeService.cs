using ICMS.MongoDBHelper;
using ICMS.RabbitMQ;
using System.Drawing;
using System.Text;
using System.Xml;
using ICMS.Contracts;
using ICMS.Contracts.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;


namespace ApplicationServices.NfeService
{
    public class NfeService : INfeService
    {
        private ICMS.RabbitMQ.IRabbitMensagemRepository _rabbitRepository;
        private MongoDBService _mongoDBService;

        public NfeService(ICMS.RabbitMQ.IRabbitMensagemRepository rabbitRepository, MongoDBService mongoDBService)
        {

            this._rabbitRepository = rabbitRepository;
            this._mongoDBService = mongoDBService;

        }
        public void sendRabbitMQMessage(RabbitMensagem mensagem, string fila)
        {
            _rabbitRepository.SendMensagem(mensagem, fila);
        }

        public void ProcessarNFE(ICMS.RabbitMQ.RabbitMensagem message)
        {
            XmlDataDocument document = new XmlDataDocument();
            document.Load(message.Texto);


            var namespaceManager = new XmlNamespaceManager(document.NameTable);
            if (document.DocumentElement.NamespaceURI != "")
                namespaceManager.AddNamespace("d", document.DocumentElement.NamespaceURI);
            else
                namespaceManager.AddNamespace("d", "");

            //nfeProc/infNFe/det
            string campo = "NFe\\infNFe\\det";
            campo = "d:" + campo.Replace(@"\", "/d:");

            XmlNodeList nodelist = document.SelectNodes(campo, namespaceManager);

            decimal TotalIcms = 0;
            foreach (XmlNode node in nodelist)
            {
                string campo2 = "imposto\\ICMS\\ICMS00";
                campo2 = "d:" + campo2.Replace(@"\", "/d:");
                var icms = node.SelectSingleNode(campo2, namespaceManager);

                decimal vBC = decimal.Parse(icms["vBC"].InnerText);
                decimal pICMS = decimal.Parse(icms["pICMS"].InnerText);

                decimal valorIcmsItem = vBC * pICMS;
                TotalIcms += valorIcmsItem;
            }

            GravarValorICMSMongo(message.Titulo, TotalIcms);

            //Thread.Sleep(5000);

            CriaCampoDivididoPor2(message.Titulo);
        }

        private async void GravarValorICMSMongo(string arquivo, decimal valor)
        {
            var doc = new BsonDocument
            {
                { "nomeArquivo", arquivo },
                { "valorIcms", new BsonDecimal128(valor) }
            };

            await _mongoDBService.InsertAsync(doc);
        }


        public async void CriaCampoDivididoPor2(string arquivo)
        {
            var pipeline = new[]
            {
                    new BsonDocument
                    {
                        { "$match", new BsonDocument("nomeArquivo", arquivo) }
                    },
                    new BsonDocument
                    {
                        { "$addFields", new BsonDocument("icmsDivided",
                            new BsonDocument("$divide",
                                new BsonArray { "$valorIcms", 2 })
                            )
                        }
                    }
                };

            try
            {
                var docs = await _mongoDBService.RunPipeline(pipeline);
                this.AtualizarICMS(arquivo, docs);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async void AtualizarICMS(string fileNameFilter, List<BsonDocument> docs)
        {

            var filter = Builders<BsonDocument>.Filter.Eq("nomeArquivo", fileNameFilter);
            try
            {
                if (docs.Count > 0)
                {
                    BsonDocument bdoc = docs.FirstOrDefault();
                    var objectId = bdoc.GetValue("_id").AsObjectId;

                    var icmsDivided = Decimal.Parse(bdoc.GetValue("icmsDivided").ToString());


                    this._mongoDBService.UpdateByIdAsync<BsonDocument>(objectId.ToString(),"icmsDivided",new Tuple<string,Type>(icmsDivided.ToString(),typeof(System.Decimal)));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }




        }

    }

    public class ValorDto
    {
        public decimal value { get; set; }  
    }
}

