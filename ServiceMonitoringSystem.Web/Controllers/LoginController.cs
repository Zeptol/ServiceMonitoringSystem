using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Security;
using FineUIMvc;
using MongoDB.Bson;
using MongoDB.Driver;
using ServiceMonitoringSystem.Common.Common;
using ServiceMonitoringSystem.IRepository;
using ServiceMonitoringSystem.Model;

namespace ServiceMonitoringSystem.Web.Controllers
{
    public class LoginController : BaseController<User>
    {
        public LoginController(IMongoRepository<User> user):base(user)
        {
            Rep = user;
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnLogin_Click(string tbxUserName, string tbxPassword)
        {
            var helper=new CommonHelper();
            var filter =
                Builders<User>.Filter.And(
                    Builders<User>.Filter.Regex(t => t.UserPwd,
                        new BsonRegularExpression(new Regex(helper.GetMd5(tbxPassword), RegexOptions.IgnoreCase))),
                    Builders<User>.Filter.Eq(t => t.UserName, tbxUserName));
            if (Rep.Get(filter) != null)
            {
                FormsAuthentication.RedirectFromLoginPage(tbxUserName, false);
            }
            else
            {
                ShowNotify("用户名或密码错误！", MessageBoxIcon.Error);
            }

            return UIHelper.Result();
        }
    }
}