using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Wechat
{
    partial class WechatLib
    {
        internal const string SendTemplateMsgUrlTemplate = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}";
        #region Sample JSON
        // {
        //    "touser":"OPENID",
        //    "template_id":"ngqIpbwh8bUfcSsECmogfXcV14J0tQlEpBO27izEYtY",
        //    "url":"http://weixin.qq.com/download",            
        //    "data":{
        //            "first": {
        //                "value":"恭喜你购买成功！",
        //                "color":"#173177"
        //            },
        //            "keynote1":{
        //                "value":"巧克力",
        //                "color":"#173177"
        //            },
        //            "keynote2": {
        //                "value":"39.8元",
        //                "color":"#173177"
        //            },
        //            "keynote3": {
        //                "value":"2014年9月22日",
        //                "color":"#173177"
        //            },
        //            "remark":{
        //                "value":"欢迎再次购买！",
        //                "color":"#173177"
        //            }
        //      }
        // }
        #endregion

        public static void SendTemplateMessage(TemplateMessageRequest message)
        {
            if (message == null)
            {
                throw new InvalidOperationException("Parameter cannot be NULL.");
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(SendTemplateMsgUrlTemplate, AccessTokenData));
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            Stream stream = request.GetRequestStream();

            string messageContent = JsonTypedSerializer<TemplateMessageRequest>.Serialize(message);
//#warning: replace with actual logic
  //              "{\"data\":{\"first\":{\"color\":\"#173177\",\"value\":\"恭喜您领票成功！\"},\"keyword1\":{\"color\":\"#173177\",\"value\":\"Test 4\"},\"keyword2\":{\"color\":\"#173177\",\"value\":\"2016-3-10\"},\"keyword3\":{\"color\":\"#173177\",\"value\":\"New Location\"},\"keyword4\":{\"color\":\"#173177\",\"value\":\"A2\"},\"remark\":{\"color\":\"#173177\",\"value\":\"点击详情可查看电子票详细信息。\"}},\"template_id\":\"sm9qjFusjXOi9Pk5CwU8G_UQVlJm8RFEFZ5XJnZay3o\",\"touser\":\"oQQZHv_-vGsgcytoYhFS00sw4mOs\",\"url\":\"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxdbb288aa372b8280&redirect_uri=https%3a%2f%2fgcsamplesale.nike.com%2fgcsamplesale%2fticket.aspx%3fticketid%3d220800c8-a434-4289-9446-1870c0764caa&response_type=code&scope=snsapi_base&state=#wechat_redirect\"}";

            DataContractJsonSerializer serRequest = new DataContractJsonSerializer(typeof(TemplateMessageRequest));
            serRequest.WriteObject(stream, message);
            stream.Close();

            HttpWebResponse res = (HttpWebResponse)request.GetResponse();
            string respondText = new StreamReader(res.GetResponseStream()).ReadToEnd();

            WechatLib.WriteLog(string.Format("Template message sent: {0}", messageContent));

            TemplateMessageRequestError error = JsonTypedSerializer<TemplateMessageRequestError>.Deserialize(respondText);
            if (error.ErrorCode != 0)
            {
                throw new InvalidOperationException(string.Format("Error: {0}|{1}", error.ErrorCode, error.MessageId));
            }
        }

        #region TemplateMessage
        [DataContract]
        public class TemplateMessageRequest
        {
            [DataMember(Name = "touser", EmitDefaultValue = true)]
            public string ToUser;

            [DataMember(Name = "template_id", EmitDefaultValue = true)]
            public string TemplateId;

            [DataMember(Name = "url", EmitDefaultValue = true)]
            public string URL;

            [DataMember(Name = "data", EmitDefaultValue = true)]
            public TemplateMessageData Data = new TemplateMessageData();

            [DataContract]
            public class TemplateMessageData
            {
                [DataMember(Name = "first", EmitDefaultValue = true)]
                public MessageText Title = new MessageText();

                [DataMember(Name = "keyword1", EmitDefaultValue = true)]
                public MessageText Keyword1 = new MessageText();

                [DataMember(Name = "keyword2", EmitDefaultValue = true)]
                public MessageText Keyword2 = null;

                [DataMember(Name = "keyword3", EmitDefaultValue = true)]
                public MessageText Keyword3 = null;

                [DataMember(Name = "keyword4", EmitDefaultValue = true)]
                public MessageText Keyword4 = null;

                [DataMember(Name = "remark", EmitDefaultValue = true)]
                public MessageText Remark = null;

                [DataContract]
                public class MessageText
                {
                    [DataMember(Name = "value", EmitDefaultValue = true)]
                    public string Value = null;

                    [DataMember(Name = "color", EmitDefaultValue = true)]
                    public string Color = null;
                }
            }
        }
        #endregion

        #region TemplateMessageRequestError
        [DataContract]
        public class TemplateMessageRequestError
        {
            [DataMember(Name = "errcode", EmitDefaultValue = true)]
            public int ErrorCode;

            [DataMember(Name = "errmsg", EmitDefaultValue = true)]
            public string ErrorMessage;

            [DataMember(Name = "msgid", EmitDefaultValue = true)]
            public string MessageId;
        }
        #endregion
    }
}
