using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FineUIMvc;
using ServiceMonitoringSystem.Model;

namespace ServiceMonitoringSystem.Interface
{
    public interface ITree
    {
        IList<TreeNode> GetTreeNodes(Expression<Func<ServiceList, bool>> filter=null);
    }
}
