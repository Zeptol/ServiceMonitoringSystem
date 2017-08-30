using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace ServiceMonitoringSystem.IRepository
{
    public interface IMongoRepository<T> where T : class
    {
        T Get(Expression<Func<T, bool>> filter);
        object Max(Expression<Func<T, object>> sort);

        List<T> QueryByPage(int pageIndex, int pageSize, out int rowCount, Expression<Func<T, bool>> filter = null,
            SortDefinition<T> sort = null);
        void Add(T model);
        T Update(Expression<Func<T, bool>> filter, UpdateDefinition<T> update);
        long Delete(Expression<Func<T, bool>> filter);
    }
}
