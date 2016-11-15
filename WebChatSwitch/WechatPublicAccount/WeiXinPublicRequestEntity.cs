using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChatPublicAccount
{
    #region Profile

    #endregion

    #region Groups

    #endregion

    #region MessageTemplate

    public class SendMessageTemplateRequest
    {
        public string touser { get; set; }
        public string template_id { get; set; }
        public string url { get; set; }
        public TemplateMessageData data { get; set; }
    }

    public class TemplateMessageData
    {
        public MessageTemplateItem first { get; set; }
        public MessageTemplateItem keyword1 { get; set; }
        public MessageTemplateItem keyword2 { get; set; }

        public MessageTemplateItem keyword3 { get; set; }

        public MessageTemplateItem keyword4 { get; set; }

        public MessageTemplateItem remark { get; set; }
    }

    public class MessageTemplateItem
    {
        public string value { get; set; }
        public string color { get; set; }
    }

    #endregion

    #region User

    public class WeiXinAttrItem
    {
        public string name { get; set; }

        public string value { get; set; }

    }

    public class WeiXinAttr
    {
        public List<WeiXinAttrItem> attrs { get; set; }
    }

    public class WeiXinUserRequest
    {
        public string userid { get; set; }
        public string name { get; set; }
        public List<int> department { get; set; }
        public string position { get; set; }
        public string mobile { get; set; }
        public int gender { get; set; }

        public string email { get; set; }
        public string weixinid { get; set; }
        public string avatar_mediaid { get; set; }

        public WeiXinAttr extattr { get; set; }

    }

    public class WeiXinUserListRequest
    {
        public List<string> useridlist { get; set; }
    }

    public class InviteUserListRequest
    {
        public string userid { get; set; }
    }

    #endregion

    #region Async Task

    public class AsyncCallback
    {
        public string url { get; set; }
        public string token { get; set; }
        public string encodingaeskey { get; set; }

    }

    public class AsyncInviteRequest
    {
        public string touser { get; set; }
        public string toparty { get; set; }
        public string totag { get; set; }

        public AsyncCallback callback { get; set; }
    }

    public class AsyncSyncContacts
    {
        public string media_id { get; set; }
        //public AsyncCallback callback { get; set; }
    }

    #endregion

    #region App

    public class UpdateAppRequest
    {
        public int agentid { get; set; }
        public int report_location_flag { get; set; }
        //public string logo_mediaid { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string redirect_domain { get; set; }
        public int isreportuser { get; set; }
        public int isreportenter { get; set; }
    }

    #endregion

    #region Message

    public class TextContent
    {
        public string content { get; set; }
    }

    public class ImageContent
    {
        public string media_id { get; set; }
    }

    public class VoiceContent
    {
        public string media_id { get; set; }
    }

    public class VedioContent
    {
        public string media_id { get; set; }
        public string title { get; set; }

        public string description { get; set; }
    }

    public class FileContent
    {
        public string media_id { get; set; }
        public string title { get; set; }

        public string description { get; set; }
    }

    public class MessageBase
    {
        public string touser { get; set; }
        public string toparty { get; set; }
        public string totag { get; set; }
        public string msgtype { get; set; }
        public string agentid { get; set; }
        public string safe { get; set; }
    }

    public class TextMessage : MessageBase
    {
        public TextContent text { get; set; }
    }

    public class ImageMessage : MessageBase
    {
        public ImageContent image { get; set; }
    }

    public class VoiceMessage : MessageBase
    {
        public VoiceContent voice { get; set; }
    }

    public class VedioMessage : MessageBase
    {
        public VedioContent video { get; set; }
    }

    public class FileMessage : MessageBase
    {
        public FileContent file { get; set; }
    }

    public class NewsItem
    {
        public string title { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string picurl { get; set; }
    }

    public class ArticleContent
    {
        public List<NewsItem> articles { get; set; }
    }

    public class ArticlesMessage : MessageBase
    {
        public ArticleContent news { get; set; }
    }



    public class mpNewsItem
    {
        public string title { get; set; }
        public string thumb_media_id { get; set; }
        public string author { get; set; }
        public string content_source_url { get; set; }

        public string content { get; set; }

        public string digest { get; set; }

        public string show_cover_pic { get; set; }
    }

    public class mpNewsArticleMetrialsContent
    {
        public string media_id { get; set; }
    }

    public class mpArticleContent
    {
        public List<mpNewsItem> articles { get; set; }
    }

    public class mpArticlesMessage : MessageBase
    {
        public mpArticleContent news { get; set; }
    }

    public class mpArticlesMetrialsMessage : MessageBase
    {
        public mpNewsArticleMetrialsContent mpnews { get; set; }
    }


    #endregion
}
