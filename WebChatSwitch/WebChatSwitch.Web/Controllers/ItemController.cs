using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebChatSwitch.BLL;
using WebChatSwitch.DAL;
using WebChatSwitch.Web.Models;
using WeChatPublicAccount;

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
                    string fileName = WeChatDownloadImage(serverId);
                    string rootPath = ConfigurationManager.AppSettings["Domain"];
                    string ftpUrl = rootPath + "/" + ConfigurationManager.AppSettings["photoFolder"] + "/" + fileName;
                    string ftpUrlForResized = rootPath + "/" + ConfigurationManager.AppSettings["resizedPhotoFolder"] + "/" + fileName;
                    if (!string.IsNullOrEmpty(ftpUrl))
                    {
                        SystemLog downlog = new SystemLog()
                        {
                            Type = "Log",
                            Content = "Saved image to " + ftpUrl,
                            Time = DateTime.UtcNow
                        };
                        logManager.AddLog(downlog);
                    }

                    ItemPicture pic = new ItemPicture()
                    {
                        PictureUrl = ftpUrl
                    };
                    item.ItemPictures.Add(pic);
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

        public ActionResult ListView(string searchString)
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

            JsInitResponse response = InitialWechatSDK(url);
            ViewBag.appId = response.appId;
            ViewBag.nonceStr = response.nonceStr;
            ViewBag.signature = response.signature;
            ViewBag.timestamp = response.timestamp;


            ItemManager manager = new ItemManager();
            List<ItemViewModel> itemList = new List<ItemViewModel>();
            List<Item> items;
            if (!string.IsNullOrEmpty(searchString))
            {
                items = manager.GetAvailableItems(searchString);
            }
            else
            {
                items = manager.GetAllAvailableItems();
            }
            foreach (var item in items)
            {
                ItemViewModel vm = new ItemViewModel();
                vm.Available = item.Available;
                List<string> photoes = new List<string>();
                foreach (ItemPicture ip in item.ItemPictures)
                {
                    photoes.Add(ip.PictureUrl);
                }
                vm.ItemPhotos = photoes;
                vm.PublishUser = item.UserAccount.Name;
                vm.PublishUserOpenId = item.UserAccount.OpenId;
                vm.Title = item.Title;
                vm.Description = item.Description;
                vm.Expectation = item.Expectation;
                itemList.Add(vm);
            }

            return View(itemList);
        }
    }
}