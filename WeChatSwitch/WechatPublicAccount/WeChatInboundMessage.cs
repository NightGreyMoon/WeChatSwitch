using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChatPublicAccount
{
    public class WeChatInboundMessage
    {
        public string ToUserName { get; set; }
        public string FromUserName { get; set; }
        public string Content { get; set; }
    }
}
