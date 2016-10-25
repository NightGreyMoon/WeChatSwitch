using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebChatSwitch.BLL;

namespace WebChatSwitch.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            UserAccountManager manager = new UserAccountManager();
            var account = manager.GetUserAccountInfoByOpenId("haobohaobohaobohaobohaobo123");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}