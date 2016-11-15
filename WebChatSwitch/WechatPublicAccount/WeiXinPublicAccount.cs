using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WeChatPublicAccount
{
    public class WeiXinPublicAccount : WeiXinAccount
    {
        public const string URL_TPL_AccessToken = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";
        public const string URL_TPL_GROUPSCREATE = "https://api.weixin.qq.com/cgi-bin/groups/create?access_token={0}";
        public const string URL_TPL_GROUPSGET = "https://api.weixin.qq.com/cgi-bin/groups/get?access_token={0}";
        public const string URL_TPL_GROUPSGETID = "https://api.weixin.qq.com/cgi-bin/groups/getid?access_token={0}";
        public const string URL_TPL_GROUPSUPDATE = "https://api.weixin.qq.com/cgi-bin/groups/update?access_token={0}";
        public const string URL_TPL_GROUPSMEMBERSUPDATE = "https://api.weixin.qq.com/cgi-bin/groups/members/update?access_token={0}";
        public const string URL_TPL_GROUPSMEMBERSBATCHUPDATE = "https://api.weixin.qq.com/cgi-bin/groups/members/batchupdate?access_token={0}";
        public const string URL_TPL_GROUPSDELETE = "https://api.weixin.qq.com/cgi-bin/groups/delete?access_token={0}";
        public const string URL_TPL_MENUCREATE = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token={0}";
        public const string URL_TPL_MENUDELETE = "https://api.weixin.qq.com/cgi-bin/menu/delete?access_token={0}";
        public const string URL_TPL_MENUADDCONDITIONAL = "https://api.weixin.qq.com/cgi-bin/menu/addconditional?access_token={0}";
        public const string URL_TPL_MENUDELCONDITIONAL = "https://api.weixin.qq.com/cgi-bin/menu/delconditional?access_token={0}";
        public const string URL_TPL_MENUTRYMATCH = "https://api.weixin.qq.com/cgi-bin/menu/trymatch?access_token={0}";
        public const string URL_TPL_MSGMASSSENDALL = "https://api.weixin.qq.com/cgi-bin/message/mass/sendall?access_token={0}";
        public const string URL_TPL_MSGTEMPLATESEND = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}";
        public const string URL_TPL_USERINFO = "https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang={2}";
        public const string URL_JS_API = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi";
        public const string URL_TPL_MEDIAGET = "http://file.api.weixin.qq.com/cgi-bin/media/get?access_token={0}&media_id={1}";

        public delegate void TokenRenew(string AccountID, string AccessToken, string JsToken);

        public event TokenRenew OnTokenRenew;
        public string jsapi_ticket
        {
            get;
            set;
        }

        public WeiXinPublicAccount(string WeiXinAppId, string WeiXinSecret)
            : base(true)
        {
            this.AccountID = WeiXinAppId;
            this.Secret = WeiXinSecret;
            //this.OnAccessTokenExpired += WeChatPublicAccount_OnAccessTokenExpired;
            //this.StartMornitor();
        }

        public WeiXinPublicAccount(string accessToken, string Cropid, bool Proxy)
            : base(false)
        {
            this.AccessToken = accessToken;
            this.AccountID = Cropid;
        }

        public int WeChatPublicAccount_OnAccessTokenExpired()
        {
            try
            {
                //access token
                var returnValue = GetAccessToken(this.AccountID, this.Secret);
                this.AccessToken = returnValue.access_token;
                this.IsAvaiable = true;
                //jsapi ticket
                this.jsapi_ticket = GetJsTicket().ticket;

                int.Parse(returnValue.expires_in);

                //if (OnTokenRenew != null)
                //{
                //    OnTokenRenew(this.AccountID, returnValue.access_token, this.jsapi_ticket);
                //}
                return 1800;
            }
            catch (Exception ex)
            {
                this.IsAvaiable = false;
                return 5;
            }
        }

        public static bool TryGetToken(string AppId, string Serce)
        {
            try
            {
                string initCommand = string.Format(URL_TPL_AccessToken, AppId, Serce);
                var returnValue = HttpClient.Get<AccessTokenResponse>(initCommand);
                int outExpired = -1;
                if (int.TryParse(returnValue.expires_in, out outExpired))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private AccessTokenResponse GetAccessToken(string AppId, string Secret)
        {
            string initCommand = string.Format(URL_TPL_AccessToken, AppId, Secret);
            return HttpClient.Get<AccessTokenResponse>(initCommand);
        }

        private JsTicketResponse GetJsTicket()
        {
            string initCommand = string.Format(URL_JS_API, this.AccessToken);
            return HttpClient.Get<JsTicketResponse>(initCommand);
        }

        public CreateGroupResponse Groups_Create(string name)
        {
            string initCommand = string.Format(URL_TPL_GROUPSCREATE, this.AccessToken);
            return HttpClient.Post<CreateGroupResponse, object>(initCommand, new { group = new { name = name } });
        }

        public GetGroupResponse Groups_Get()
        {
            string initCommand = string.Format(URL_TPL_GROUPSGET, this.AccessToken);
            return HttpClient.Get<GetGroupResponse>(initCommand);
        }

        public GetGroupIdResponse Groups_GetId(string openId)
        {
            string initCommand = string.Format(URL_TPL_GROUPSGETID, this.AccessToken);
            return HttpClient.Post<GetGroupIdResponse, object>(initCommand, new { openid = openId });
        }

        public GeneralResponse Groups_Update(int id, string name)
        {
            string initCommand = string.Format(URL_TPL_GROUPSUPDATE, this.AccessToken);
            return HttpClient.Post<GeneralResponse, object>(initCommand, new { group = new { id = id, name = name } });
        }

        public GeneralResponse Groups_MembersUpdate(string openId, int toGroupId)
        {
            string initCommand = string.Format(URL_TPL_GROUPSMEMBERSUPDATE, this.AccessToken);
            return HttpClient.Post<GeneralResponse, object>(initCommand, new { openid = openId, to_groupid = toGroupId });
        }

        public GeneralResponse Groups_MembersBatchUpdate(string[] openIdList, int toGroupId)
        {
            string initCommand = string.Format(URL_TPL_GROUPSMEMBERSBATCHUPDATE, this.AccessToken);
            return HttpClient.Post<GeneralResponse, object>(initCommand, new { openid_list = openIdList, to_groupid = toGroupId });
        }

        public GeneralResponse Groups_Delete(int groupId)
        {
            string initCommand = string.Format(URL_TPL_GROUPSDELETE, this.AccessToken);
            return HttpClient.Post<GeneralResponse, object>(initCommand, new { group = new { id = groupId } });
        }

        public GeneralResponse Menu_Create(string menuJsonString)
        {
            string initCommand = string.Format(URL_TPL_MENUCREATE, this.AccessToken);
            return HttpClient.Post<GeneralResponse>(initCommand, menuJsonString);
        }

        public GeneralResponse Menu_Delete()
        {
            string initCommand = string.Format(URL_TPL_MENUDELETE, this.AccessToken);
            return HttpClient.Get<GeneralResponse>(initCommand);
        }

        public MenuConditionalResponse Menu_AddConditional(string menuJsonString)
        {
            string initCommand = string.Format(URL_TPL_MENUADDCONDITIONAL, this.AccessToken);
            return HttpClient.Post<MenuConditionalResponse>(initCommand, menuJsonString);
        }

        public MenuConditionalResponse Menu_DelConditional()
        {
            string initCommand = string.Format(URL_TPL_MENUDELCONDITIONAL, this.AccessToken);
            return HttpClient.Post<MenuConditionalResponse>(initCommand, null);
        }

        public string Menu_TryMatch(string openIdOrWeixinId)
        {
            string initCommand = string.Format(URL_TPL_MENUTRYMATCH, this.AccessToken);
            return HttpClient.Post<string, object>(initCommand, new { user_id = openIdOrWeixinId });
        }

        public SendMessageTemplateResponse Message_TemplateSend(SendMessageTemplateRequest templateRequest)
        {
            string initCommand = string.Format(URL_TPL_MSGTEMPLATESEND, this.AccessToken);
            return HttpClient.Post<SendMessageTemplateResponse, SendMessageTemplateRequest>(initCommand, templateRequest);
        }
        public UserResponse User_Info(string openId, string lan = "zh-CN")
        {
            string initCommand = string.Format(URL_TPL_USERINFO, this.AccessToken, openId, lan);
            return HttpClient.Get<UserResponse>(initCommand);
        }

        /// <summary>
        /// 回复多图文消息
        /// </summary>
        /// <param name="toUserName"></param>
        /// <param name="fromUserName"></param>
        /// <param name="news"></param>
        /// <returns></returns>
        public static string RepayNews(string toUserName, string fromUserName, List<NewsItem> news)
        {
            var builder = new StringBuilder();
            builder.Append(string.Format("<xml><ToUserName><![CDATA[{0}]]></ToUserName>" +
            "<FromUserName><![CDATA[{1}]]></FromUserName>" +
            "<CreateTime>{2}</CreateTime>" +
            "<MsgType><![CDATA[news]]></MsgType>" +
            "<ArticleCount>{3}</ArticleCount><Articles>",
             toUserName, fromUserName,
             Util.CreateTimestamp(),
             news.Count
                ));
            foreach (var c in news)
            {
                builder.Append(string.Format("<item><Title><![CDATA[{0}]]></Title>" +
                    "<Description><![CDATA[{1}]]></Description>" +
                    "<PicUrl><![CDATA[{2}]]></PicUrl>" +
                    "<Url><![CDATA[{3}]]></Url>" +
                    "</item>",
                   c.title, c.description, c.picurl, c.url
                 ));
            }
            builder.Append("</Articles></xml>");
            return builder.ToString();
        }

        /// <summary>
        /// 回复文本消息
        /// </summary>
        /// <param name="toUserName"></param>
        /// <param name="fromUserName"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RepayText(string toUserName, string fromUserName, string content)
        {
            return string.Format("<xml><ToUserName><![CDATA[{0}]]></ToUserName>" +
                                                   "<FromUserName><![CDATA[{1}]]></FromUserName>" +
                                                   "<CreateTime>{2}</CreateTime>" +
                                                   "<MsgType><![CDATA[text]]></MsgType>" +
                                                   "<Content><![CDATA[{3}]]></Content></xml>",
                                                   toUserName, fromUserName, Util.CreateTimestamp(), content);
        }

        public string Media_GetBase64(string mediaId)
        {
            string initCommand = string.Format(URL_TPL_MEDIAGET, this.AccessToken, mediaId);
            var bytes = new WebClient().DownloadData(initCommand);
            return Convert.ToBase64String(bytes);
        }
    }
}
