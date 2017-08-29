using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace ServiceMonitoringSystem.IRepository
{
    public interface IMongoRepository<T> where T : class
    {
        T Get(IMongoQuery query);

        List<T> QueryByPage(int pageIndex, int pageSize, out int rowCount, IMongoQuery where = null,
            SortByBuilder sort = null);
    }
}
