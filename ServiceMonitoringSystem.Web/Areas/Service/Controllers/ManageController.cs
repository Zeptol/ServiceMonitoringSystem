using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using ServiceMonitoringSystem.IRepository;
using ServiceMonitoringSystem.Model;
using ServiceMonitoringSystem.Web.Controllers;

namespace ServiceMonitoringSystem.Web.Areas.Service.Controllers
{
    public class ManageController : BaseController<ServiceEntity>
    {
        public ManageController(IMongoRepository<ServiceEntity> rep):base(rep)
        {
            Rep = rep;
            Updated += ManageController_Updated;
        }

        private void ManageController_Updated(NameValueCollection values)
        {
            var name = values["tbxName"];
            var filter = new List<FilterDefinition<ServiceEntity>>();
            if (!string.IsNullOrEmpty(name))
                filter.Add(Builders<ServiceEntity>.Filter.Regex(t => t._id,
                    new BsonRegularExpression(new Regex(name, RegexOptions.IgnoreCase))));
            UpdateGrid(values, filter);
        }

        //
        // GET: /Service/Manage/
        public override ActionResult Index()
        {
            return base.Index();
        }

        public ActionResult Delete()
        {
            throw new NotImplementedException();
        }
    }
}