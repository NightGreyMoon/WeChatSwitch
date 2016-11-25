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
                    Type = "Error",
                    Content = string.Format("Failed to get OpenId, requested url:{0}", url),
                    Time = DateTime.UtcNow
                };
                logManager.AddLog(log);
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