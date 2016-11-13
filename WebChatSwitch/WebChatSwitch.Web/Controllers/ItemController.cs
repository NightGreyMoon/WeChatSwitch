using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebChatSwitch.BLL;
using WebChatSwitch.DAL;
using WebChatSwitch.Web.Models;
using WechatPublicAccount;

namespace WebChatSwitch.Web.Controllers
{
    public class ItemController : WeChatBaseController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);


        }

        // GET: Item
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            string url = Request.Url.ToString();
            string code = Request.Params["code"];
            //判断是否有OpenId
            if (CurrentUser == null || string.IsNullOrWhiteSpace(CurrentUser.OpenId))
            {
                LogManager logManager = new LogManager();
                SystemLog log = new SystemLog()
                {
                    Type = "Log",
                    Content = string.Format("Get url and code, url:{0}, code:{1}", url, code),
                    Time = DateTime.UtcNow
                };
                logManager.AddLog(log);


                string appId = ConfigurationManager.AppSettings["AppID"];
                string appSecret = ConfigurationManager.AppSettings["AppSecret"];

                var client = new System.Net.WebClient();
                client.Encoding = System.Text.Encoding.UTF8;

                var requestUrl = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code", appId, appSecret, code);
                var data = client.DownloadString(requestUrl);

                var serializer = new JavaScriptSerializer();
                var obj = serializer.Deserialize<Dictionary<string, string>>(data);
                string openId;
                if (!obj.TryGetValue("openid", out openId))
                {
                    SystemLog failLog = new SystemLog()
                    {
                        Type = "Log",
                        Content = string.Format("Can not Get openId"),
                        Time = DateTime.UtcNow
                    };
                    logManager.AddLog(failLog);
                }
                else
                {
                    SystemLog resultLog = new SystemLog()
                    {
                        Type = "Log",
                        Content = string.Format("Get openId, openId:{0}", openId),
                        Time = DateTime.UtcNow
                    };
                    logManager.AddLog(resultLog);

                    UserAccountManager uaManager = new UserAccountManager();
                    UserAccount account = uaManager.GetUserAccountInfoByOpenId(openId);

                    CurrentUser = new LoginUser() { Id = account.Id, OpenId = openId };
                }
            }

            ItemViewModel vm = new ItemViewModel();
            vm.Available = true;
            vm.ItemPhotos = new List<string>();
            vm.ItemPhotos.Add(string.Empty);
            vm.ItemPhotos.Add(string.Empty);
            vm.ItemPhotos.Add(string.Empty);

            JsInitResponse response = InitialWechatSDK(url);
            ViewBag.appId = response.appId;
            ViewBag.nonceStr = response.nonceStr;
            ViewBag.signature = response.signature;
            ViewBag.timestamp = response.timestamp;

            return View(vm);
        }

        [HttpPost]
        public ActionResult Create(string Title, string Description, string Expectation, string Available, string[] ServerIds)
        {
            LogManager logManager = new LogManager();
            SystemLog log = new SystemLog()
            {
                Type = "Log",
                Content = string.Format("Item Create Action post called.Title:{0}, Description{1}, Expectation{2}, Available:{3}, ServerId1:{4}",
                    Title, Description, Expectation, Available, ServerIds == null ? "null" : ServerIds[0]),
                Time = DateTime.UtcNow
            };
            logManager.AddLog(log);

            bool result;
            if (bool.TryParse(Available, out result))
            {
                Item item = new Item()
                {
                    Title = Title,
                    Description = Description,
                    Expectation = Expectation,
                    Available = result,
                    PublishedTime = DateTime.Now
                };

                if (CurrentUser != null)
                {
                    item.OwnerId = CurrentUser.Id;
                }

                foreach (var serverId in ServerIds)
                {
                    string referUrl = WeixinDownloadImage(serverId);
                    SystemLog downlog = new SystemLog()
                    {
                        Type = "Log",
                        Content = "Save image to " + referUrl,
                        Time = DateTime.UtcNow
                    };
                    logManager.AddLog(downlog);
                }

                ItemManager manager = new ItemManager();
                if (manager.SaveNewItem(item))
                {
                    return Content("true");
                }
                else
                {
                    SystemLog dblog = new SystemLog()
                    {
                        Type = "Log",
                        Content = "Failed to save Item to DB.",
                        Time = DateTime.UtcNow
                    };
                    logManager.AddLog(dblog);
                    return Content("false");
                }
            }
            else
            {
                SystemLog parseFailLog = new SystemLog()
                {
                    Type = "Log",
                    Content = "Failed to parse Available.",
                    Time = DateTime.UtcNow
                };
                logManager.AddLog(parseFailLog);
                return Content("false");
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

        //获取临时素材
        //https://api.weixin.qq.com/cgi-bin/media/get?access_token=ACCESS_TOKEN&media_id=MEDIA_ID 

        //微信下载图片
        public string WeixinDownloadImage(string mediaId)
        {
            string accessToken = string.Empty;
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
            accessToken = cache.Token;

            LogManager logManager = new LogManager();
            SystemLog log = new SystemLog()
            {
                Type = "Log",
                Content = "Token got, Token: " + accessToken,
                Time = DateTime.UtcNow
            };
            logManager.AddLog(log);

            string result = string.Empty;
            try
            {
                var response = HttpClient.Get(string.Format("https://api.weixin.qq.com/cgi-bin/media/get?access_token={0}&media_id={1}", accessToken, mediaId));
                var stream = response.GetResponseStream();
                Bitmap bitmap = new Bitmap(stream);
                string basePath = "/UploadPic/";
                string filename = DateTime.Now.Ticks + ".jpg";

                Bitmap bm2 = new Bitmap(bitmap.Width, bitmap.Height);
                Graphics g = Graphics.FromImage(bm2);
                g.DrawImageUnscaled(bitmap, 0, 0);
                bm2.Save(Server.MapPath(basePath) + filename);

                //返回地址
                string path = ConfigurationManager.AppSettings["Domain"].ToString();
                result = path + basePath + filename;
            }
            catch (Exception ex)
            {
                SystemLog exLog = new SystemLog()
                {
                    Type = "Exception",
                    Content = ex.Message,
                    Time = DateTime.UtcNow
                };
                logManager.AddLog(exLog);
            }
            return result;
        }
    }
}