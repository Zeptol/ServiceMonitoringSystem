using System.Web.Mvc;
using System.Web.Security;
using FineUIMvc;

namespace ServiceMonitoringSystem.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Hello()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnHello_Click()
        {
            Alert.Show("你好 ！", MessageBoxIcon.Warning);

            return UIHelper.Result();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult onSignOut_Click()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Login");
        }



        // GET: Themes
        public ActionResult Themes()
        {
            return View();
        }
    }
}