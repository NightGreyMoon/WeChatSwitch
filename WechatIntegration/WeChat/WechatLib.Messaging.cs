using System;

namespace Wechat
{
    partial class WechatLib
    {
        public static Func<WechatEventMessage, string, WechatBaseMessage> EventMessageHandler = delegate { return new WechatTextMessage(); };

        public static Func<WechatTextMessage, string, WechatBaseMessage> TextMessageHandler = delegate { return new WechatTextMessage(); };

        public static Func<WechatImageMessage, string, WechatBaseMessage> ImageMessageHandler = delegate { return new WechatTextMessage(); };

        public static Func<WechatBaseMessage, string, WechatBaseMessage> UnknownMessageHandler = delegate { return new WechatTextMessage(); };

        public static WechatBaseMessage RespondMessage(WechatBaseMessage inboundMessage, string rawXml)
        {
            WechatBaseMessage outboundMessage;

            //Create message object of specific type based on MsgType value
            switch (inboundMessage.MsgType.ToLower())
            {
                case "text":
                    WechatTextMessage itm = XmlTypedSerializer<WechatTextMessage>.Deserialize(rawXml);
                    WriteLog("text message received: " + itm.Content, DateTime.UtcNow);
                    outboundMessage = TextMessageHandler(itm, rawXml);
                    break;
                case "event":
                    WechatEventMessage iem = XmlTypedSerializer<WechatEventMessage>.Deserialize(rawXml);
                    WriteLog(string.Format("event message received: {0}|{1}", iem.EventType, iem.EventKey), DateTime.UtcNow);
                    outboundMessage = EventMessageHandler(iem, rawXml);
                    break;
                case "image":
                    WechatImageMessage iim = XmlTypedSerializer<WechatImageMessage>.Deserialize(rawXml);
                    WriteLog(string.Format("image message received: {0}", iim.PicUr), DateTime.UtcNow);
                    outboundMessage = ImageMessageHandler(iim, rawXml);
                    break;
                default:
                    WriteLog(string.Format("Unsupported message type: {0}", inboundMessage.MsgType), DateTime.UtcNow);
                    outboundMessage = UnknownMessageHandler(inboundMessage, rawXml);
                    break;
            }

            return outboundMessage;
        }
    }
}