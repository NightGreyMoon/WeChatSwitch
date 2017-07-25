using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChatSwitch.Web.Models
{
    public class UserInfo
    {
        public string OpenId { get; set; }

        public string UserName { get; set; }

        public string Nickname { get; set; }

        public string WeChatAccount { get; set; }

        public string SelfIntroduction { get; set; }

        public string SurplusPublishNumber { get; set; }
    }
}