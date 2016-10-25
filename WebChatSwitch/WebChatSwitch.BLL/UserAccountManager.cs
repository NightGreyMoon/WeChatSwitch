using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebChatSwitch.DAL;

namespace WebChatSwitch.BLL
{
    public class UserAccountManager
    {
        private WebChatSwitchEntities context;

        public WebChatSwitchEntities Context
        {
            get
            {
                return context;
            }

            set
            {
                context = value;
            }
        }
        public UserAccountManager()
        {
            Context = new WebChatSwitchEntities();
        }

        public UserAccount GetUserAccountInfoByOpenId(string openId)
        {
            var userAccount = Context.UserAccounts.Include("Items").Include("Items.ItemPictures").FirstOrDefault(us => us.OpenId == openId);
            return userAccount;
        }
    }
}
