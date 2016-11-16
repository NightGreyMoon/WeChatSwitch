using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebChatSwitch.DAL;

namespace WebChatSwitch.BLL
{
    public class UserAccountManager : BaseManager
    {

        public UserAccountManager()
        {
            Context = new WebChatSwitchEntities();
        }

        public UserAccount GetUserAccountInfoByOpenId(string openId)
        {
            UserAccount userAccount = Context.UserAccounts.FirstOrDefault(ua => ua.OpenId == openId);
            return userAccount;
        }

        public void SaveUserAccountInfo(UserAccount userAccount)
        {
            UserAccount entity = Context.UserAccounts.FirstOrDefault(ua => ua.OpenId == userAccount.OpenId);
            entity.WeChatNickName = userAccount.WeChatNickName;
            entity.Remark = userAccount.Remark;
            Context.SaveChanges();
        }

        public int UpdateUserDisplayNameByOpenId(string openId, string name)
        {
            UserAccount entity = Context.UserAccounts.FirstOrDefault(ua => ua.OpenId == openId);
            if (entity == null)
                return -1;
            entity.Name = name;
            int result = Context.SaveChanges();
            return result;
        }

        public int UpdateUserWeChatNumberByOpenId(string openId, string wcNo)
        {
            UserAccount entity = Context.UserAccounts.FirstOrDefault(ua => ua.OpenId == openId);
            if (entity == null)
                return -1;
            entity.WeChatNumber = wcNo;
            int result = Context.SaveChanges();
            return result;
        }

        public bool InsertUserAccountByOpenId(UserAccount userAccount)
        {
            UserAccount entity = Context.UserAccounts.FirstOrDefault(ua => ua.OpenId == userAccount.OpenId);
            if (entity == null)
            {
                Context.UserAccounts.Add(userAccount);
                int result = Context.SaveChanges();
                return result > 0;
            }
            return false;
        }
    }
}
