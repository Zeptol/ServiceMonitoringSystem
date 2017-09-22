using System.Collections.Generic;
using FineUIMvc;

namespace ServiceMonitoringSystem.Interface
{
    public interface ICommon
    {
        IList<ListItem> GetTypeList();
        IList<ListItem> GetTypeSelectList(string typeName);
    }
}
