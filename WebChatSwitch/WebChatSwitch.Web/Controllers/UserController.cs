using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebChatSwitch.BLL;
using WebChatSwitch.DAL;
using WebChatSwitch.Web.Models;

namespace WebChatSwitch.Web.Controllers
{
    public class UserController : WeChatBaseController
    {
        #region ViewUserInfo
        public ActionResult ViewUserInfo()
        {
            string queryOpenId = Request.QueryString["openid"];

            //todo get date form database
            UserAccountManager manager = new UserAccountManager();

            if (string.IsNullOrWhiteSpace(queryOpenId))
            {
                queryOpenId = CurrentUser.OpenId;
            }

            UserAccount userInfo = manager.GetUserAccountInfoByOpenId(queryOpenId);
            userInfo.WeChatNumber = userInfo.WeChatNumber;
            userInfo.OpenId = CurrentUser.OpenId;               //get open id
            userInfo.Name = userInfo.Name;
            userInfo.WeChatNickName = userInfo.WeChatNickName;
            userInfo.Remark = userInfo.Remark;
            userInfo.Balance = userInfo.Balance;

            return View(userInfo);
        }
        #endregion

        #region ViewMyItem
        public ActionResult ViewMyItem()
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
            return View();
        }

        public ActionResult GetMyItem()
        {
            List<ItemViewModel> itemList = new List<ItemViewModel>();

            try
            {
                ItemManager manager = new ItemManager();

                List<Item> items;
                LogManager logManager = new LogManager();
                //SystemLog log = new SystemLog();
                //log.Type = "Log";
                //log.Content = $"[View My Item] User Id: { CurrentUser.Id}; Oped Id: { CurrentUser.OpenId}";
                //log.Time = DateTime.UtcNow;
                //logManager.AddLog(log);
                items = manager.GetMyItems(CurrentUser.Id);
                foreach (var item in items)
                {
                    ItemViewModel vm = new ItemViewModel();
                    vm.Id = item.Id;
                    vm.Available = item.Available;
                    List<string> photoes = new List<string>();
                    foreach (ItemPicture ip in item.ItemPictures)
                    {
                        photoes.Add(ip.PictureUrl);
                    }
                    vm.ItemPhotos = photoes;
                    vm.PublishUser = item.UserAccount.Name;
                    vm.Title = item.Title;
                    vm.Description = item.Description;
                    vm.Expectation = item.Expectation;
                    itemList.Add(vm);
                }
            }
            catch (Exception ex)
            {
                LogManager logManager = new LogManager();
                SystemLog log = new SystemLog();
                log.Type = "Log";
                log.Content = $"[View My Item] Page Error: { ex.Message }";
                log.Time = DateTime.UtcNow;
                logManager.AddLog(log);
            }

            return Json(itemList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SoldOut()
        {
            string id = Request.Form["id"];

            int intId;

            if (!int.TryParse(id, out intId))
            {
                return Json("Id not number");
            }

            ItemManager manager = new ItemManager();
            manager.SoldOut(intId);

            return Json("Succeed");
        }
        #endregion

        #region ViewEditMyInfo

        public ActionResult ViewEditMyInfo()
        {
            return View();
        }

        public ActionResult GetMyInfo()
        {
            UserAccount userInfo = new UserAccount();
            UserAccountManager manager = new UserAccountManager();
            userInfo = manager.GetUserAccountInfoByOpenId(CurrentUser.OpenId);
            return Json(userInfo, JsonRequestBehavior.AllowGet); 
        }

        public ActionResult SaveMyInfo()
        {
            string opedId = Request.Form["OpenId"];
            string weChatAccount = Request.Form["WeChatAccount"];
            string name = Request.Form["Name"];
            string remark = Request.Form["SelfIntroduction"];

            UserAccount userInfo = new UserAccount();
            if (!string.IsNullOrWhiteSpace(opedId))
            {
                userInfo.OpenId = opedId;
                userInfo.WeChatNumber = weChatAccount;
                userInfo.Name = name;
                userInfo.Remark = remark;

                UserAccountManager manager = new UserAccountManager();
                manager.SaveUserAccountInfo(userInfo);
            }

            return Json("Save succeed!");
        }
        #endregion




    }
}