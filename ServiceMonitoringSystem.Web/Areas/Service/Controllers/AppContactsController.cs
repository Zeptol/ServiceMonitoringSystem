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
    public class AppContactsController : BaseController<AppContacts>
    {
        public AppContactsController(IMongoRepository<AppContacts> rep):base(rep)
        {
            Rep = rep;
            Updated += AppContactsController_Updated;
        }

        private void AppContactsController_Updated(NameValueCollection values)
        {
            var name = values["tbxName"];
            var filter = new List<FilterDefinition<AppContacts>>();
            if (!string.IsNullOrEmpty(name))
                filter.Add(Builders<AppContacts>.Filter.Regex(t => t._id,
                    new BsonRegularExpression(new Regex(name, RegexOptions.IgnoreCase))));
            UpdateGrid(values, filter);
        }

	}
}