using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WebChatSwitch.BLL;
using WebChatSwitch.DAL;
using WebChatSwitch.Web.Models;

namespace WebChatSwitch.Web.Controllers
{
    public class UserController : WeChatBaseController
    {
        // GET: User
        public ActionResult ViewUserInfo()
        {
            //todo get date form database
            UserAccountManager manager = new UserAccountManager();

            UserAccount userInfo = manager.GetUserAccountInfoByOpenId(CurrentUser.OpenId);
            userInfo.WeChatNumber = userInfo.WeChatNumber;
            userInfo.OpenId = CurrentUser.OpenId;               //get open id
            userInfo.Name = userInfo.Name;             
            userInfo.WeChatNickName = userInfo.WeChatNickName;             
            userInfo.Remark = userInfo.Remark;     
            userInfo.Balance = userInfo.Balance; 

            return View(userInfo);
        }

        public ActionResult SaveUserInfo()
        {
            UserAccount userInfo = new UserAccount();
            if (CurrentUser == null || string.IsNullOrWhiteSpace(CurrentUser.OpenId))
            {
                userInfo.OpenId = Request.Form["hideOpenId"];
                userInfo.WeChatNumber = Request.Form["lbWeChatAccount"];
                userInfo.Name = Request.Form["lbUserName"];
                userInfo.WeChatNickName = Request.Form["txtNickName"];
                userInfo.Remark = Request.Form["txtSelfIntroduction"];
                userInfo.Balance = short.Parse(Request.Form["lbSurplusPublishNumber"]);

                UserAccountManager manager = new UserAccountManager();
                manager.SaveUserAccountInfo(userInfo);
            }

            return View(userInfo);
        }

        public ActionResult ViewMyItem()
        {
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
                SystemLog log = new SystemLog();
                log.Content = $"[View My Item] User Id: { CurrentUser.Id}; Oped Id: { CurrentUser.OpenId}";
                log.Time = DateTime.UtcNow;
                logManager.AddLog(log);
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
            catch(Exception ex)
            {
                LogManager logManager = new LogManager();
                SystemLog log = new SystemLog();
                log.Content = $"[View My Item] Page Error: { ex.Message }";
                log.Time = DateTime.UtcNow;
                logManager.AddLog(log);
            }
            
            return Json(itemList,JsonRequestBehavior.AllowGet);
        }

        public ActionResult SoldOut()
        {
            string id = Request.Form["id"];

            int intId;

            if(!int.TryParse(id,out intId))
            {
                return Json("Id not number");
            }

            ItemManager manager = new ItemManager();
            manager.SoldOut(intId);

            return Json("Succeed");
        }
    }
}