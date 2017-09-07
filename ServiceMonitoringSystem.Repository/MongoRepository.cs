using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using ServiceMonitoringSystem.IRepository;

namespace ServiceMonitoringSystem.Repository
{
    public class MongoRepository<T> : IMongoRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(string collectionName = null)
        {
            _collection = GetCollection(collectionName);
        }

        private static IMongoCollection<T> GetCollection(string collectionName)
        {
            var mongoUrl = new MongoUrl(ConfigurationManager.AppSettings["mongo"]);
            var mongoClient = new MongoClient(mongoUrl);
            var database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            return database.GetCollection<T>(collectionName ?? typeof(T).Name);
        }

        public T Get(Expression<Func<T, bool>> filter)
        {
            return _collection.Find(filter).FirstOrDefault();
        }

        public T Get(FilterDefinition<T> filter)
        {
            return _collection.Find(filter).FirstOrDefault();
        }

        public object Max(Expression<Func<T, object>> sort)
        {
            var res = _collection.Find(new BsonDocument()).SortByDescending(sort).Limit(1);
            return res.ToList().Select(sort.Compile()).FirstOrDefault();
        }

        public List<T> QueryByPage(int pageIndex, int pageSize, out int rowCount, FilterDefinition<T> filter,
            SortDefinition<T> sort)
        {
            var queryFilter = filter ?? new FilterDefinitionBuilder<T>().Empty;
            rowCount = (int)_collection.Count(queryFilter);
            var res = _collection.Find(queryFilter);
            if (sort != null)
                res = res.Sort(sort);
            res = res.Skip(pageSize * pageIndex).Limit(pageSize);
            return res.ToList();
        }

        public void Add(T model)
        {
            _collection.InsertOne(model);
        }

        public T Update(Expression<Func<T, bool>> filter, UpdateDefinition<T> update)
        {
            var res = _collection.FindOneAndUpdate(filter, update);
            return res;
        }

        public long Delete(Expression<Func<T, bool>> filter)
        {
            var res = _collection.DeleteMany(filter);
            return res.IsAcknowledged ? res.DeletedCount : 0;
        }
    }
}
