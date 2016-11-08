using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wechat;

namespace Wechat
{
    public class WechatMessageHandlerBasePage : System.Web.UI.Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            BindEventToMessageDelegates();
            HandleWechatRequest();
        }

        private void BindEventToMessageDelegates()
        {
            WechatLib.TextMessageHandler = OnHandleTextMessage;
            WechatLib.EventMessageHandler = OnHandleEventMessage;
            WechatLib.ImageMessageHandler = OnHandleImageMessage;
            WechatLib.UnknownMessageHandler = OnHandleUnknownMessage;
        }

        private void HandleWechatRequest()
        {
            string method = Request.HttpMethod.ToUpper();

            WechatLib.WriteLog(method + "|" + Request.Url, DateTime.UtcNow);

            switch (method)
            {
                case "GET":
                    // 微信加密签名    
                    string signature = Request.QueryString["signature"];
                    // 时间戳    
                    string timestamp = Request.QueryString["timestamp"];
                    // 随机数    
                    string nonce = Request.QueryString["nonce"];
                    // 随机字符串    
                    string echostr = Request.QueryString["echostr"];
                    if (WechatLib.CheckSignature(signature, timestamp, nonce))
                    {
                        Response.ClearContent();
                        Response.ContentType = "text/plain";
                        Response.Write(echostr);
                        WechatLib.WriteLog("Echo:" + echostr, DateTime.UtcNow);
                    }
                    Response.End();
                    break;
                case "POST":
                    Response.ContentType = "application/xml";
                    StreamReader stream = new StreamReader(Request.InputStream);
                    string xml = stream.ReadToEnd();
                    WechatBaseMessage msg = WechatBaseMessage.FromXml<WechatBaseMessage>(xml);
                    WechatLib.WriteMessageLog(msg, WechatLib.MessageDirection.Inbound, xml);

                    Response.ClearContent();

                    WechatBaseMessage responseMsg = WechatLib.RespondMessage(msg, xml);

                    if (responseMsg != null)
                    {
                        responseMsg.CreateTime = DateTime.UtcNow;
                        responseMsg.ToUserName = msg.FromUserName;
                        responseMsg.FromUserName = msg.ToUserName;

                        string responseText = responseMsg.ToXml();
                        Response.Write(responseText);
                        WechatLib.WriteMessageLog(responseMsg, WechatLib.MessageDirection.Outbound, responseText);
                    }
                    Response.End();
                    break;
            }
        }

        protected virtual WechatBaseMessage OnHandleEventMessage(WechatEventMessage arg, string rawXml)
        {
            using( WechatMessageHandlerSvc.WechatSvcClient client = new WechatMessageHandlerSvc.WechatSvcClient())
            {
                return client.HandleEventMessage(arg, rawXml);
            }
        }

        protected virtual WechatBaseMessage OnHandleTextMessage(WechatTextMessage arg, string rawXml)
        {
            using (WechatMessageHandlerSvc.WechatSvcClient client = new WechatMessageHandlerSvc.WechatSvcClient())
            {
                return client.HandleTextMessage(arg, rawXml);
            }
        }

        protected virtual WechatBaseMessage OnHandleImageMessage(WechatImageMessage arg, string rawXml)
        {
            using (WechatMessageHandlerSvc.WechatSvcClient client = new WechatMessageHandlerSvc.WechatSvcClient())
            {
                return client.HandleImageMessage(arg, rawXml);
            }
        }

        protected virtual WechatBaseMessage OnHandleUnknownMessage(WechatBaseMessage arg, string rawXml)
        {
            using (WechatMessageHandlerSvc.WechatSvcClient client = new WechatMessageHandlerSvc.WechatSvcClient())
            {
                return client.HandleUnknownMessage(arg, rawXml);
            }
        }
    }
}