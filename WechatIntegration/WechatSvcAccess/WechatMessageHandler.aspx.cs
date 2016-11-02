using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wechat;

namespace WechatSvcHub
{
    public partial class WechatMessageHandler : WechatMessageHandlerBasePage
    {
        protected override Wechat.WechatBaseMessage OnHandleEventMessage(Wechat.WechatEventMessage arg, string rawXml)
        {
            return new WechatTextMessage() { Content = "event message" };
        }

        protected override Wechat.WechatBaseMessage OnHandleTextMessage(Wechat.WechatTextMessage arg, string rawXml)
        {
            return new WechatTextMessage() { Content = arg.FromUserName };
        }

        protected override Wechat.WechatBaseMessage OnHandleImageMessage(Wechat.WechatImageMessage arg, string rawXml)
        {
            return new WechatTextMessage() { Content = "image message" };
        }

    }
}