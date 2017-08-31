using System;
using System.Web.Mvc;
using System.Web.Security;
using FineUIMvc;
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
            if (_user.Get(t => t.UserName == tbxUserName &&
                               t.UserPwd.Equals(
                                   FormsAuthentication.HashPasswordForStoringInConfigFile(tbxPassword, "md5"),
                                   StringComparison.InvariantCultureIgnoreCase)) != null)
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