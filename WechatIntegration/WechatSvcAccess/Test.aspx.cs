using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wechat;

namespace WechatSvcHub
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            WechatLib.TemplateMessageRequest request = new WechatLib.TemplateMessageRequest
            {
                ToUser = "oQQZHv_-vGsgcytoYhFS00sw4mOs",
                TemplateId = "sm9qjFusjXOi9Pk5CwU8G_UQVlJm8RFEFZ5XJnZay3o",
                URL = "http://wechathubqa.azurewebsites.net/test.aspx",
                Data = new WechatLib.TemplateMessageRequest.TemplateMessageData()
                {
                    Title = new WechatLib.TemplateMessageRequest.TemplateMessageData.MessageText()
                    {
                        Value = "恭喜您领票成功！",
                        Color = "#173177"
                    },
                    Keyword1 = new WechatLib.TemplateMessageRequest.TemplateMessageData.MessageText()
                    {
                        Value = "New Event",
                        Color = "#173177"
                    },
                    Keyword2 = new WechatLib.TemplateMessageRequest.TemplateMessageData.MessageText()
                    {
                        Value = "2016-3-10",
                        Color = "#173177"
                    },
                    Keyword3 = new WechatLib.TemplateMessageRequest.TemplateMessageData.MessageText()
                    {
                        Value = "New Location",
                        Color = "#173177"
                    },
                    Keyword4 = new WechatLib.TemplateMessageRequest.TemplateMessageData.MessageText()
                    {
                        Value = "###票号###",
                        Color = "#173177"
                    },
                    Remark = new WechatLib.TemplateMessageRequest.TemplateMessageData.MessageText()
                    {
                        Value = "点击详情可查看电子票详细信息。",
                        Color = "#173177"
                    }
                }
            };
            WechatLib.SendTemplateMessage(request);
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
        }
    }
}