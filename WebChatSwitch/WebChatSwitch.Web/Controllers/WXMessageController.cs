using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wechat;

namespace WebChatSwitch.Web.Controllers
{
    public class WXMessageController : Controller
    {      
               
        public ActionResult WXCallback(string signature, string timestamp, string nonce, string echostr)
        {
            if (this.Request.HttpMethod.ToLower() == "get")
            {
                string token = ConfigurationManager.AppSettings["Token"];
                string encodingAESKey = ConfigurationManager.AppSettings["EncodingAESKey"];
                string appId = ConfigurationManager.AppSettings["AppID"];

                string encryptStr = Wechat.WeChatCrypter.GenarateSignature(token, timestamp, nonce);
                if (encryptStr.ToLower() == signature.ToLower())
                {
                    return Content(echostr);
                }
                else
                {
                    return Content("error");
                }
            }
            else
            {
                return Content("error");
                //string resp = "";
                //string sReqData = string.Empty;
                //using (var bodyStream = new StreamReader(this.Request.InputStream))
                //{
                //    bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
                //    sReqData = bodyStream.ReadToEnd();
                //}

                //using (var reader = new MemoryStream(Encoding.UTF8.GetBytes(sReqData)))
                //{
                //    var msg = (TencentMessage)serializer.Deserialize(reader);
                //    string MessageID = string.Empty;
                //    if (!string.IsNullOrEmpty(msg.MsgId))
                //    {
                //        MessageID = msg.MsgId;
                //    }
                //    else
                //    {
                //        MessageID = msg.FromUserName + msg.CreateTime;
                //    }
                //    ServiceBusMgr.RecieveMessage(MessageID, JsonConvert.SerializeObject(msg));

                //    resp = ProcessWeixinMessage(msg);
                //}

                //return new ContentResult
                //{
                //    Content = resp,
                //    ContentType = "text/xml",
                //    ContentEncoding = System.Text.UTF8Encoding.UTF8
                //};

                //decrpt message
                //int ret = 0;
                //WXBizMsgCrypt wxcpt = new WXBizMsgCrypt(token, encodingAESKey, appId);
                //string sMsg = "";
                //ret = wxcpt.DecryptMsg(signature, timestamp, nonce, sReqData, ref sMsg);
                //if (ret != 0)
                //{
                //    Response.Write("error");
                //    return;
                //}
                //else
                //{
                //    try
                //    {
                //        using (var reader = new MemoryStream(Encoding.UTF8.GetBytes(sMsg)))
                //        {
                //            var msg = (TencentMessage)serializer.Deserialize(reader);
                //            string MessageID = string.Empty;
                //            if (!string.IsNullOrEmpty(msg.MsgId))
                //            {
                //                MessageID = msg.MsgId;
                //            }
                //            else
                //            {
                //                MessageID = msg.FromUserName + msg.CreateTime;
                //            }
                //            ServiceBusMgr.RecieveMessage(MessageID, JsonConvert.SerializeObject(msg));
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        Response.Write(ex.ToString());
                //    }

                //    return;
                //}
            }
        }

        //private string ProcessWeixinMessage(Wechat.WechatBaseMessage msg)
        //{
        //    switch (msg.MsgType)
        //    {
        //        case "text":
        //            break;
        //        case "image":
        //            return ProcessUserAvatar(msg);
        //        case "voice":
        //            break;
        //        case "video":
        //            break;
        //        case "shortvideo":
        //            break;
        //        case "location":
        //            break;
        //        case "link":
        //            break;
        //        case "event":
        //            switch (msg.Event)
        //            {
        //                case "subscribe"://订阅
        //                    return WelcomeInfo(msg);
        //                case "unsubscribe"://取消订阅

        //                    break;
        //                case "SCAN"://用户已关注时的事件推送
        //                    break;
        //                case "LOCATION":
        //                    break;
        //                case "CLICK":
        //                    if (msg.EventKey == "badges")
        //                    {
        //                        return ProcessBadgesClick(msg);
        //                    }
        //                    break;
        //                case "VIEW":
        //                    break;
        //                default:
        //                    break;
        //            }
        //            break;
        //        default:
        //            break;
        //    }

        //    return "success";
        //}

        //private string ProcessBadgesClick(TencentMessage msg)
        //{
        //    var openId = msg.FromUserName;
        //    using (var db = new MeetingLightDB())
        //    {
        //        var profile = db.UserProfiles.FirstOrDefault(p => p.UserIdentity == openId && p.Status == Enumerator.UserProfileStatus.Active.ToString());
        //        if (profile == null)
        //        {
        //            return "success";
        //        }

        //        var sql = @"select b.BadgeID,b.EventID,b.GroupCode,b.BadgeGroup,b.Color,sg.StartDate,
        //                            sg.EndDate,sg.GifImage as BageImage,b.BgImage,sg.HeaderImage Logo 
        //                    from UserGroup ug,Badge b
        //                    left join SessionGroup sg on b.EventID=sg.EventID and b.GroupCode = sg.GroupCode
        //                     where ug.UserProfileID = '{0}'
        //                     and ug.EventID is not null and ug.EventID = b.EventID
        //                     and ug.GroupCode = b.GroupCode and ug.BadgeGroup = b.BadgeGroup
        //                     and convert(varchar(10),getDate(),120) between convert(varchar(10),b.StartDate,120) and convert(varchar(10),b.EndDate,120)+' 23:59:59'
        //                     order by b.StartDate";

        //        sql = string.Format(sql, profile.UserProfileID);

        //        var badges = db.Database.SqlQuery<Badge>(sql).ToList();

        //        //var eventList = db.UserGroups.Where(p => p.UserProfileID == profile.UserProfileID &&
        //        //    p.Status == Enumerator.UserGroupStatus.Confirm.ToString())
        //        //    .Select(s => s.EventID).Distinct().ToList();
        //        //var badges = db.Badges.Where(p => eventList.Contains(p.EventID) && p.StartDate < DateTime.Now && p.EndDate > DateTime.Now)
        //        //    .ToList();

        //        //var badgeEventList = badges.Select(s => s.EventID).Distinct().ToList();

        //        //var badgeList = new List<Badge>();
        //        //foreach (Guid? eventId in badgeEventList)
        //        //{
        //        //    var badge = badges.Where(p => p.EventID == eventId).OrderBy(o => o.StartDate).FirstOrDefault();
        //        //    badgeList.Add(badge);
        //        //}

        //        var news = new List<NewsItem>();
        //        var title = "";
        //        if (profile.Language == "en")
        //        {
        //            title = "Badge List";
        //        }
        //        else
        //        {
        //            title = "通行证列表";
        //        }
        //        news.Add(new NewsItem
        //        {
        //            title = title,
        //            url = ConfigHelper.Read("MiniSite")
        //        });
        //        int i = 0;
        //        foreach (var b in badges)
        //        {
        //            if (i == 10) break;
        //            news.Add(new NewsItem
        //            {
        //                title = b.SessionGroup.SessionGroupCode,
        //                url = ConfigHelper.Read("MiniSite") + "/Home/Ebadge?eventid=" + b.SessionGroup.EventID.ToString()
        //            });
        //            i++;
        //        }
        //        return WeiXinPublicAccount.RepayNews(openId, msg.ToUserName, news);
        //    }
        //}

        //private string WelcomeInfo(TencentMessage msg)
        //{
        //    var news = new List<NewsItem>();
        //    news.Add(new NewsItem
        //    {
        //        title = "欢迎关注耐克销售会议管理公众号",
        //        url = ""
        //    });
        //    news.Add(new NewsItem
        //    {
        //        title = "选择中文注册",
        //        url = ConfigHelper.Read("MiniSite") + "/language/switch?lan=zh-cn"
        //    });
        //    news.Add(new NewsItem
        //    {
        //        title = "Choose English For Registration",
        //        url = ConfigHelper.Read("MiniSite") + "/language/switch?lan=en"
        //    });

        //    return WeiXinPublicAccount.RepayNews(msg.FromUserName, msg.ToUserName, news);
        //}

        //private string ProcessUserAvatar(TencentMessage msg)
        //{
        //    var lan = "zh-CN";
        //    var openId = msg.FromUserName;

        //    try
        //    {
        //        using (var db = new MeetingLightDB())
        //        {
        //            UserProfile profile = null;
        //            var profiles = db.UserProfiles.Where(p => p.UserIdentity == openId && p.Status == Enumerator.UserProfileStatus.Active.ToString()).ToList();
        //            if (profiles.Count() > 1 || profiles.Count() == 0)
        //            {
        //                lan = profiles[0].Language;
        //                return WeiXinPublicAccount.RepayText(openId, msg.ToUserName, lan == "en" ?
        //                    "Your account occur an error,please contact administrator." : "您的账户出现异常,请联系管理员.");
        //            }
        //            else
        //            {
        //                profile = profiles[0];
        //            }

        //            //判断当前用户参加的会议是否有active的
        //            var activeEventCount = db.UserGroups.Where(p => p.UserProfileID == profile.UserProfileID &&
        //                p.Status == Enumerator.UserGroupStatus.Confirm.ToString() && p.Enabled &&
        //                p.EventList.Status == Enumerator.EventListStatus.Active.ToString()).Count();
        //            if (activeEventCount > 0)
        //            {
        //                return WeiXinPublicAccount.RepayText(openId, msg.ToUserName, lan == "en" ?
        //                        "You can't edit your avatar, during your meeting is active" :
        //                        "不能修改您的头像, 因为您所参加会议已经开始");
        //            }

        //            lan = string.IsNullOrWhiteSpace(profile.Language) ? lan : profile.Language;

        //            var httpClient = new HttpClient();
        //            var bytes = httpClient.GetByteArrayAsync(msg.PicUrl).Result;
        //            var webImageHelper = new WebImageHelper(bytes);
        //            bytes = webImageHelper.Resize(350, 350).GetBytes();
        //            var suffix = "";
        //            if (msg.PicUrl.IndexOf(".png") > -1) suffix = ".png";
        //            else suffix = ".jpg";

        //            var picUrl = Common.Util.UploadImage(bytes, suffix);

        //            log.DebugFormat("上传图片的微信地址为[{0}],azure地址为[{1}]", msg.PicUrl, picUrl);

        //            profile.Logo = picUrl;

        //            db.SaveChanges();
        //            return WeiXinPublicAccount.RepayText(openId, msg.ToUserName, lan == "en" ?
        //                "Upload your avatar success" : "上传头像成功");

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.DebugFormat("获取到用户[{0}]的头像,头像微信地址为[{1}]",
        //            openId, msg.PicUrl);
        //        log.Error("上传用户头像异常", ex);
        //        return WeiXinPublicAccount.RepayText(openId, msg.ToUserName, lan == "en" ?
        //            "Upload your avatar fail" : "上传头像失败");
        //    }
        //}

        public ActionResult JSInit(string currentURL)
        {

            string jsToken = "";
            string AccountID = ConfigurationManager.AppSettings["AppId"];

            var nonceStr = "Wm3WZYTPz0wzccnW";
            var timestamp = DateTime.Now.Ticks.ToString();
            var source = string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", jsToken, nonceStr, timestamp, currentURL.Split("#".ToArray(), StringSplitOptions.RemoveEmptyEntries)[0]);
            JsInitResponse result = new JsInitResponse()
            {
                appId = AccountID,
                nonceStr = nonceStr,
                signature = Wechat.WeChatCrypter.GenarateSignature(source),
                timestamp = timestamp
            };
            return Json(result);
        }

        //public ActionResult GetWeixinImageBase64(string serverId)
        //{
        //    try
        //    {
        //        string URL_TPL_MEDIAGET = "http://file.api.weixin.qq.com/cgi-bin/media/get?access_token={0}&media_id={1}";

        //        string accessToken = "";
        //        using (var db = new MeetingLightDB())
        //        {
        //            var profile = db.WeChatProfiles.FirstOrDefault();
        //            accessToken = profile.AccessToken;
        //        }
        //        string url = string.Format(URL_TPL_MEDIAGET, accessToken, serverId);
        //        var httpClient = new HttpClient();
        //        var content = httpClient.GetAsync(url).Result;

        //        IEnumerable<string> contentTypes;
        //        content.Headers.TryGetValues("Content-Type", out contentTypes);
        //        string contentType = (contentTypes == null || string.IsNullOrEmpty(contentTypes.FirstOrDefault())) ? "image/jpeg" : contentTypes.FirstOrDefault();
        //        var bytes = content.Content.ReadAsByteArrayAsync().Result;

        //        return Json("data:" + contentType + ";base64," + Convert.ToBase64String(bytes),
        //            JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("获取图片异常", ex);
        //        return Json("error", JsonRequestBehavior.AllowGet);
        //    }
        //}
    }
}