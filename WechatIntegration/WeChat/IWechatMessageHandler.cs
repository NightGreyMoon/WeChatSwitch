using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Wechat;

namespace Wechat
{
    [ServiceContract]
    public interface IWechatMessageHandler
    {
        [OperationContract]
        WechatBaseMessage HandleEventMessage(WechatEventMessage arg, string rawXml);

        [OperationContract]
        WechatBaseMessage HandleTextMessage(WechatTextMessage arg, string rawXml);

        [OperationContract]
        WechatBaseMessage HandleImageMessage(WechatImageMessage arg, string rawXml);

        [OperationContract]
        WechatBaseMessage HandleUnknownMessage(WechatBaseMessage arg, string rawXml);
    }
}
