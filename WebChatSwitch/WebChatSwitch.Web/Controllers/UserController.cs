using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebChatSwitch.BLL;
using WebChatSwitch.DAL;
using WebChatSwitch.Web.Models;

namespace WebChatSwitch.Web.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult ViewUserInfo()
        {
            //todo get date form database
            UserAccountManager manager = new UserAccountManager();

            UserAccount userInfo = manager.GetUserAccountInfoByOpenId("");
            userInfo.WeChatNumber = "";
            userInfo.OpenId = "";               //get open id
            userInfo.Name = "";             
            userInfo.WeChatNickName = "";             
            userInfo.Remark = "";     
            userInfo.Balance = 1; 

            return View();
        }

        public ActionResult SaveUserInfo()
        {
            UserAccount userInfo = new UserAccount();

            userInfo.OpenId = Request.Form["hideOpenId"];
            userInfo.WeChatNumber = Request.Form["lbWeChatAccount"];
            userInfo.Name = Request.Form["txtUserName"];
            userInfo.WeChatNickName = Request.Form["txtNickName"];
            userInfo.Remark = Request.Form["txtSelfIntroduction"];
            userInfo.Balance = short.Parse(Request.Form["lbSurplusPublishNumber"]);

            //todo save entity to database


            return View(userInfo);
        }
    }
}