using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Security;
using FineUIMvc;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using ServiceMonitoringSystem.IRepository;
using ServiceMonitoringSystem.Model;

namespace ServiceMonitoringSystem.Web.Controllers
{
    public class LoginController : BaseController
    {
        private readonly IMongoRepository<User> _user;
        public LoginController(IMongoRepository<User> user)
        {
            _user = user;
        }

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnLogin_Click(string tbxUserName, string tbxPassword)
        {
            var query = Query.And(Query<User>.EQ(t => t.UserName, tbxUserName),
                Query<User>.Matches(t => t.UserPwd,
                    new BsonRegularExpression(
                        new Regex(FormsAuthentication.HashPasswordForStoringInConfigFile(tbxPassword, "md5"),
                            RegexOptions.IgnoreCase))));
            if (_user.Get(query)!=null)
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