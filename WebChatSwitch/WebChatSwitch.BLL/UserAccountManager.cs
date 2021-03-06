﻿using System;
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
            var userAccount = Context.UserAccounts.Include("Items").Include("Items.ItemPictures").FirstOrDefault(ua => ua.OpenId == openId);
            return userAccount;
        }
    }
}
