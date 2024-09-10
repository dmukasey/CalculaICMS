
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Drawing;
using ICMS.Contracts.Models;
using static MongoDB.Driver.WriteConcern;

namespace ICMS.MongoDBHelper
{


    public class MongoDBService
    {
        private readonly IMongoCollection<BsonDocument> _collection;

        public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            var client = new MongoClient(mongoDBSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _collection = database.GetCollection<BsonDocument>(mongoDBSettings.Value.CollectionName);
        }

        public async Task InsertAsync(BsonDocument item)
        {
            await _collection.InsertOneAsync(item);
        }

        public async Task<BsonDocument> GetByIdAsync(string id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<BsonDocument>> GetAllAsync()
        {
            return await _collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task UpdateByIdAsync<BsonDocument>(string id, string campo, Tuple<string,Type> valor) 
        {
            try
            {
                UpdateDefinition<MongoDB.Bson.BsonDocument> update = null;
                var filter = Builders<MongoDB.Bson.BsonDocument>.Filter.Eq("_id", new ObjectId(id));
                
                if (valor.Item2 == typeof(System.Decimal))
                {
                    update = Builders<MongoDB.Bson.BsonDocument>.Update.Set(campo, decimal.Parse(valor.Item1));
                }
                else if (valor.Item2.GetType() == typeof(int))
                {
                    update = Builders<MongoDB.Bson.BsonDocument>.Update.Set(campo, int.Parse(valor.Item1));
                }
                else
                {
                    update = Builders<MongoDB.Bson.BsonDocument>.Update.Set(campo, valor);
                }
                await _collection.UpdateOneAsync(filter, update);
            }
            catch (Exception ex)
            {
                throw ex;
            }
           

        }

       

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Id", id);
            await _collection.DeleteOneAsync(filter);
        }


        public async Task<List<BsonDocument>> RunPipeline(BsonDocument[] pipeline)
        {
            try
            {
                var result = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync();                
                
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
       
    }
}


    public class MongoDBSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
    }
    


