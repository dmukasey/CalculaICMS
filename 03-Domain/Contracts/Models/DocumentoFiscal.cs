using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Contracts.Models
{
    public class DocumentoFiscal
    {
        public string nomeArquivo {  get; set; }
        public decimal valorIcms {  get; set; }
        public decimal icmsDivided { get; set; }
        

        /* public static explicit operator global::MongoDB.Bson.BsonDocument(DocumentoFiscal v)
         {
             throw new NotImplementedException();
         }*/
    }
}
