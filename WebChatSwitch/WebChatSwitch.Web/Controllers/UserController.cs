using System.Web.Mvc;
using WebChatSwitch.BLL;
using WebChatSwitch.DAL;

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
    }
}