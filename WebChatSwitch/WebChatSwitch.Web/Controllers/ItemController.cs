using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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

            JsInitResponse response = InitialWechatSDK("http://scrumoffice.azurewebsites.net/Item/Create");
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
                    cache.Timestamp = DateTime.UtcNow;
                    manager.UpdateWechatCache(cache);                   
                }
            }
            jsToken = cache.Ticket;

            string nonceStr = "Wm3WZYTPz0wzccnW";
            string timestamp = CreatenTimestamp().ToString();


            string source = string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", jsToken, nonceStr, timestamp, currentURL);
            //string signature = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(source, "SHA1");

            //string source = "jsapi_ticket=kgt8ON7yVITDhtdwci0qeTQIB0ds82IxTu3pDvW_gUqD60Q9zCrv1YkvovovbWdlJzFtIPryUYqDTrPQ1Sk6ww&noncestr=Wm3WZYTPz0wzccnW&timestamp=1414587457&url=http://webchatswitch.azurewebsites.net/Item/Create";

            //建立SHA1对象
            SHA1 sha = new SHA1CryptoServiceProvider();
            //将mystr转换成byte[] 
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] dataToHash = enc.GetBytes(source);
            //Hash运算
            byte[] dataHashed = sha.ComputeHash(dataToHash);
            //将运算结果转换成string
            string signature = BitConverter.ToString(dataHashed).Replace("-", "").ToLower();

            

            JsInitResponse result = new JsInitResponse()
            {
                appId = AppId,
                nonceStr = nonceStr,
                signature = signature,
                timestamp = timestamp
            };
            //string source = string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", jsToken, nonceStr, timestamp, currentURL.Split("#".ToArray(), StringSplitOptions.RemoveEmptyEntries)[0]);
            //JsInitResponse result = new JsInitResponse()
            //{
            //    appId = AppId,
            //    nonceStr = nonceStr,
            //    signature = WeChatCrypter.GenarateSignature(source),
            //    timestamp = timestamp
            //};
            return result;
        }

        public static long CreatenTimestamp()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }
    }
}