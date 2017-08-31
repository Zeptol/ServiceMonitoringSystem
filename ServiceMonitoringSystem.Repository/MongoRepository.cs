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
        public MongoRepository()
        {
            _collection = GetCollection();
        }
        private static IMongoCollection<T> GetCollection(string collectionName = null)
        {
            var mongoUrl = new MongoUrl(ConfigurationManager.AppSettings["mongo"]);
            var mongoClient = new MongoClient(mongoUrl);
            var database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            return database.GetCollection<T>(collectionName ?? typeof(T).Name);
        }
        public T Get(Expression<Func<T, bool>> filter)
        {
            return _collection.FindOneAndUpdate(filter, null);
            //return _collection.Find(filter).Limit(1) as T;
        }
        public object Max(Expression<Func<T, object>> sort)
        {
            var cursor = _collection.Find(new BsonDocument()).SortByDescending(sort).Limit(1);
            return cursor.ToList().Select(sort.Compile()).FirstOrDefault();
        }
        public List<T> QueryByPage(int pageIndex, int pageSize, out int rowCount, Expression<Func<T, bool>> filter, SortDefinition<T> sort)
        {
            rowCount = (int)_collection.Count(filter);
            var res = _collection.Find(filter);
            if (sort!=null)
                res=res.Sort(sort);
            res = res.Skip(pageSize * pageIndex).Limit(pageSize);
            return res.ToList();
        }

        public void Add(T model)
        {
            //if (typeof(T).GetProperty("Rid") != null)
            //{
            //    var list = _collection.FindAs<T>(null)
            //        .SetSortOrder(SortBy.Descending("Rid"))
            //        .SetLimit(1).ToList();
            //    typeof(T).GetProperty("Rid").SetValue(model, 1, null);
            //    if (list.Count > 0)
            //    {
            //        var rId = (int)list.First().GetType().GetProperty("Rid").GetValue(list.First(), null);
            //        typeof(T).GetProperty("Rid").SetValue(model, rId + 1, null);
            //    }
            //    else
            //    {
            //        typeof(T).GetProperty("Rid").SetValue(model, 1, null);
            //    }
            //}
            _collection.InsertOne(model);
        }

        public T Update(Expression<Func<T, bool>> filter, UpdateDefinition<T> update)
        {
            //IMongoQuery query = Query.EQ("Rid", (int)typeof(T).GetProperty("Rid").GetValue(model, null));
            //var modelDb = Get(query);
            //var id = modelDb.GetType().GetProperty("_id").GetValue(modelDb, null);
            //typeof(T).GetProperty("_id").SetValue(model, id, null);
            //model._id = list.First()._id;
            var res = _collection.FindOneAndUpdate(filter, update);
            return res;
        }

        public long Delete(Expression<Func<T, bool>> filter)
        {
            var res = _collection.DeleteOne(filter);
            return res.DeletedCount;
        }
    }
}
