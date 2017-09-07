using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using FineUIMvc;
using ServiceMonitoringSystem.Interface;

namespace ServiceMonitoringSystem.Service
{
    public class CommonService:ICommon
    {
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
    }
}
