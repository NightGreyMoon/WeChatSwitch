using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebChatSwitch.BLL;
using WebChatSwitch.DAL;
using WebChatSwitch.Web.Models;
using WechatPublicAccount;

namespace WebChatSwitch.Web.Controllers
{
    public class ItemController : Controller
    {
        // GET: Item
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            ItemViewModel vm = new ItemViewModel();
            vm.Available = true;
            vm.ItemPhotos = new List<string>();
            vm.ItemPhotos.Add(string.Empty);
            vm.ItemPhotos.Add(string.Empty);
            vm.ItemPhotos.Add(string.Empty);

            JsInitResponse response = InitialWechatSDK("http://webchatswitch.azurewebsites.net/Item/Create");
            ViewBag.appId = response.appId;
            ViewBag.nonceStr = response.nonceStr;
            ViewBag.signature = response.signature;
            ViewBag.timestamp = response.timestamp;

            return View(vm);
        }

        [HttpPost]
        public ActionResult Create(string Title, string Description, string Expectation, string Photo1)
        {
            if (string.IsNullOrEmpty(Photo1))
            {
                return Content("");
            }
            else
            {
                return Content("true");
            }
        }


        public JsInitResponse InitialWechatSDK(string currentURL)
        {

            string jsToken = string.Empty;
            string AppId = ConfigurationManager.AppSettings["AppId"];
            string AppSecret = ConfigurationManager.AppSettings["AppSecret"];
            WeiXinPublicAccount wxpa = new WeiXinPublicAccount(AppId, AppSecret);

            WechatManager manager = new WechatManager();
            WechatCache cache = manager.GetWechatCache();
            if (cache == null)
            {
                cache = new WechatCache();
                int returnCode = wxpa.WeChatPublicAccount_OnAccessTokenExpired();
                if (returnCode == 1800)
                {
                    cache.Token = wxpa.AccessToken;
                    cache.Ticket = wxpa.jsapi_ticket;
                    cache.Timestamp = DateTime.Now;
                    manager.UpdateWechatCache(cache);

                    jsToken = wxpa.jsapi_ticket;
                }
            }

            string nonceStr = "Wm3WZYTPz0wzccnW";
            string timestamp = DateTime.Now.Ticks.ToString();
            string source = string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", jsToken, nonceStr, timestamp, currentURL.Split("#".ToArray(), StringSplitOptions.RemoveEmptyEntries)[0]);
            JsInitResponse result = new JsInitResponse()
            {
                appId = AppId,
                nonceStr = nonceStr,
                signature = WeChatCrypter.GenarateSignature(source),
                timestamp = timestamp
            };
            return result;
        }
    }
}