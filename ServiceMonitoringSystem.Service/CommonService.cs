using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using FineUIMvc;
using MongoDB.Driver;
using ServiceMonitoringSystem.Interface;
using ServiceMonitoringSystem.IRepository;
using ServiceMonitoringSystem.Model;

namespace ServiceMonitoringSystem.Service
{
    public class CommonService:ICommon
    {
        private readonly IMongoRepository<BasicType> _rep;

        public CommonService(IMongoRepository<BasicType> rep)
        {
            _rep = rep;
        }
        public IList<ListItem> GetTypeList()
        {
            try
            {
                var typeArr = ConfigurationManager.AppSettings["BasicTypes"].Split(new[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries);
                return typeArr.Select((t, i) => new ListItem
                {
                    Text = t,
                    Value = (i + 1).ToString()
                }).ToArray();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public IList<ListItem> GetTypeSelectList(string typeName)
        {
            var typeList = GetTypeList();
            if (!typeList.Select(t => t.Text).Contains(typeName))
            {
                throw new ArgumentException("无效的类型名称");
            }
            var type = typeList.First(t => t.Text == typeName);
            var list = _rep.Find().ToList();
            return list.Where(t => t.TypeId == int.Parse(type.Value)).Select(t => new ListItem
            {
                Text = t.Name,
                Value = t.Num.ToString()
            }).ToArray();
        }

    }
}
