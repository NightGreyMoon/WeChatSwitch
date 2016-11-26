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
            //判断是否有OpenId
            if (CurrentUser == null || string.IsNullOrWhiteSpace(CurrentUser.OpenId))
            {
                LogManager logManager = new LogManager();
                SystemLog log = new SystemLog()
                {
                    Type = "Error",
                    Content = string.Format("Failed to get OpenId, requested url:{0}", url),
                    Time = DateTime.UtcNow
                };
                logManager.AddLog(log);
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
            //判断是否有OpenId
            if (CurrentUser == null || string.IsNullOrWhiteSpace(CurrentUser.OpenId))
            {
                LogManager logManager = new LogManager();
                SystemLog log = new SystemLog()
                {
                    Type = "Error",
                    Content = string.Format("Failed to get OpenId, requested url:{0}", url),
                    Time = DateTime.UtcNow
                };
                logManager.AddLog(log);
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
                vm.Expectation = string.IsNullOrEmpty(item.Expectation) ? null : " | " + item.Expectation;
                itemList.Add(vm);
            }

            return View(itemList);
        }
    }
}