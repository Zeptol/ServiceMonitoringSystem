using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using ServiceMonitoringSystem.IRepository;

namespace ServiceMonitoringSystem.Repository
{
    public class MongoRepository<T> : IMongoRepository<T> where T : class
    {
        private readonly MongoCollection _collection;
        public MongoRepository()
        {
            _collection = new MongoClient(ConfigurationManager.AppSettings["mongo"]).GetServer().GetDatabase(ConfigurationManager.AppSettings["dbName"]).GetCollection(typeof(T).Name);
        }
        public T Get(IMongoQuery query)
        {
            return _collection.FindOneAs<T>(query);
        }

        public List<T> QueryByPage(int pageIndex, int pageSize, out int rowCount, IMongoQuery where = null, SortByBuilder sort = null)
        {
            rowCount = (int) _collection.Count();
            var cursor = _collection.FindAs<T>(where);
            if (sort!=null)
                cursor.SetSortOrder(sort);
            cursor.SetSkip(pageSize * (pageIndex - 1));
            cursor.SetLimit(pageSize);
            return cursor.ToList();
        }
    }
}
