using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.MongoDBHelper
{
    public interface IMongoService<T> where T : class
    {

        Task InsertAsync(T item);


        Task<T> GetByIdAsync(string id);


        Task<IEnumerable<T>> GetAllAsync();


        Task UpdateAsync(string id, T item);

        Task DeleteAsync(string id);

    }
}


