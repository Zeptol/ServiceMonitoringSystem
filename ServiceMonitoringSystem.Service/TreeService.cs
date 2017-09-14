using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FineUIMvc;
using MongoDB.Driver;
using ServiceMonitoringSystem.Interface;
using ServiceMonitoringSystem.IRepository;
using ServiceMonitoringSystem.Model;

namespace ServiceMonitoringSystem.Service
{
    public class TreeService:ITree
    {
        private readonly IMongoRepository<ServiceEntity> _rep;

        public TreeService(IMongoRepository<ServiceEntity> rep)
        {
            _rep = rep;
        }

        public IList<TreeNode> GetTreeNodes(Expression<Func<ServiceEntity, bool>> filter)
        {
            var list = _rep.Find(filter).ToList();
            var all = new TreeNode {Text = "全部", NodeID = "all", Expanded = true};
            var primary = list.GroupBy(s => new {s.PrimaryId, s.ServiceName})
                .Select(g => new {g.First().PrimaryId, g.First().ServiceName}).ToList();
            foreach (var p in primary)
            {
                var tree = new TreeNode {NodeID = "1_" + p.PrimaryId, Text = p.ServiceName};
                var secondary = list.Where(t => t.PrimaryId == p.PrimaryId).ToList();
                foreach (var sec in secondary)
                {
                    tree.Nodes.Add(new TreeNode {NodeID = "2_" + sec.SecondaryId, Text = sec.SecondaryName});
                }
                all.Nodes.Add(tree);
            }
            return new[] {all};
        }
    }
}
