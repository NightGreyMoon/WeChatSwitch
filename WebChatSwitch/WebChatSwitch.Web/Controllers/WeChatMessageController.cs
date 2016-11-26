using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using WebChatSwitch.BLL;
using WebChatSwitch.DAL;
using Wechat;
using WeChatPublicAccount;

namespace WebChatSwitch.Web.Controllers
{
    public class WeChatMessageController : WeChatBaseController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            WechatLib.TextMessageHandler = OnHandleTextMessage;
            WechatLib.EventMessageHandler = OnHandleEventMessage;
            WechatLib.ImageMessageHandler = OnHandleImageMessage;
            WechatLib.UnknownMessageHandler = OnHandleUnknownMessage;
        }

        public ActionResult WeChaCallback(string signature, string timestamp, string nonce, string echostr)
        {
            if (this.Request.HttpMethod.ToLower() == "get")
            {
                string token = ConfigurationManager.AppSettings["Token"];
                string encodingAESKey = ConfigurationManager.AppSettings["EncodingAESKey"];
                string appId = ConfigurationManager.AppSettings["AppID"];

                LogManager manager = new LogManager();
                SystemLog log = new SystemLog()
                {
                    Type = "Log",
                    Content = "WeChat Verify Message Received.",
                    Time = DateTime.UtcNow
                };
                manager.AddLog(log);

                string encryptStr = WeChatCrypter.GenarateSignature(token, timestamp, nonce);
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
                LogManager manager = new LogManager();
                SystemLog log = new SystemLog()
                {
                    Type = "Log",
                    Content = "WeChat Message Received.",
                    Time = DateTime.UtcNow
                };
                manager.AddLog(log);
                //return Content("error");
                string responseText = "";
                StreamReader stream = new StreamReader(Request.InputStream);
                string xml = stream.ReadToEnd();
                WechatBaseMessage msg = WechatBaseMessage.FromXml<WechatBaseMessage>(xml);
                WechatLib.WriteMessageLog(msg, WechatLib.MessageDirection.Inbound, xml);

                SystemLog log2 = new SystemLog()
                {
                    Type = "Log",
                    Content = "WeChat Message Received." + xml,
                    Time = DateTime.UtcNow
                };
                manager.AddLog(log2);

                WechatBaseMessage responseMsg = WechatLib.RespondMessage(msg, xml);

                if (responseMsg != null)
                {
                    responseMsg.CreateTime = DateTime.UtcNow;
                    responseMsg.ToUserName = msg.FromUserName;
                    responseMsg.FromUserName = msg.ToUserName;

                    responseText = responseMsg.ToXml();

                    WechatLib.WriteMessageLog(responseMsg, WechatLib.MessageDirection.Outbound, responseText);
                }

                return new ContentResult
                {
                    Content = responseText,
                    ContentType = "text/xml",
                    ContentEncoding = System.Text.UTF8Encoding.UTF8
                };

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

        protected WechatBaseMessage OnHandleEventMessage(Wechat.WechatEventMessage arg, string rawXml)
        {
            LogManager manager = new LogManager();
            SystemLog log = new SystemLog()
            {
                Type = "Log",
                Content = "WeChat Event Message Received.",
                Time = DateTime.UtcNow
            };
            manager.AddLog(log);
            switch (arg.EventType.ToUpper())
            {
                //If the user has not subscribed the service account in the past, then the scan event type is SUBSCRIBE;
                //Otherwise scan event type is SCAN.
                case "SUBSCRIBE":
                    //Save user information to Table UserAccount
                    SystemLog subscribeLog = new SystemLog()
                    {
                        Type = "Log",
                        Content = "Subscribe Event Message Received, rawXML:" + rawXml,
                        Time = DateTime.UtcNow
                    };
                    manager.AddLog(subscribeLog);
                    WeChatInboundMessage message = Util.ParseEventXMLMessage(rawXml);
                    string fromUerOpenId = message.FromUserName;
                    bool saved = WeChatGetUserBasicInfoByOpenId(fromUerOpenId);
                    if (saved)
                    {
                        return new WechatTextMessage()
                        {
                            Content = "Thanks for subscribing us, please repley your name with 'Name:' for us to remember your name, e.g. [Name:Jason Ding]. || " +
                            "感谢关注此公众号，请以文字消息回复您的姓名，系统将把它作为对您的称呼显示在您发布的物品的上面，注意加上英文前缀'Name:'，如[Name:Jason Ding]。"
                        };
                    }
                    else
                    {
                        return new WechatTextMessage()
                        {
                            Content = "Thanks for subscribing us, but we failed to create account for you, please contact support. || " +
                            "感谢关注此公众号，创建账号失败，可能因为已有账号，请联系系统支持。"
                        };
                    }
                case "SCAN":
                    #region Bind&ScanTicket
                    //if (arg.EventKey.Contains("BIND_USER|"))
                    //{
                    //    string userUniqueName = arg.EventKey.Substring(arg.EventKey.IndexOf("BIND_USER|")).Replace("BIND_USER|", "").Trim();
                    //    using (SampleSalesDatabaseDataContext database = new SampleSalesDatabaseDataContext())
                    //    {
                    //        UserBinding binding = database.UserBindings.SingleOrDefault(b => b.WechatOpenId == arg.FromUserName);
                    //        if (binding == null)
                    //        {
                    //            binding = new UserBinding()
                    //            {
                    //                WechatOpenId = arg.FromUserName,
                    //                UserUniqueName = userUniqueName,
                    //                BindDate = DateTime.Now
                    //            };
                    //            database.UserBindings.InsertOnSubmit(binding);
                    //            database.SubmitChanges();

                    //            WechatLib.WriteLog(string.Format("Bind user success: {0}|{1}", userUniqueName, arg.FromUserName));

                    //            if (PhotoLogic.UserBadgePhotoExists(userUniqueName))
                    //            {
                    //                return new WechatTextMessage() { Content = "Welcome to Sample Sales. \r\nUser ID: " + userUniqueName };
                    //            }
                    //            else
                    //            {
                    //                return new WechatTextMessage() { Content = string.Format("Welcome to Sample Sales.\r\nUser ID: {0}.\r\nPlease take a picture of your security badge and send on Wechat.", userUniqueName) };
                    //            }
                    //        }
                    //        else
                    //        {
                    //            return new WechatTextMessage() { Content = "User ID: " + binding.UserUniqueName };
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    int sceneId = 0;
                    //    if (int.TryParse(arg.EventKey, out sceneId))
                    //    {
                    //        using (SampleSalesDatabaseDataContext database = new SampleSalesDatabaseDataContext())
                    //        {
                    //            SalesEventGroup eventGroup = database.SalesEventGroups.SingleOrDefault(g => g.EventGroupId == sceneId);
                    //            if (eventGroup != null)
                    //            {
                    //                EventAdminBinding adminBinding = new EventAdminBinding()
                    //                {
                    //                    EventGroupId = eventGroup.EventGroupId,
                    //                    WechatOpenId = arg.FromUserName
                    //                };
                    //                if (database.EventAdminBindings.FirstOrDefault(eab => eab.EventGroupId == eventGroup.EventGroupId && eab.WechatOpenId == arg.FromUserName) == null)
                    //                {
                    //                    database.EventAdminBindings.InsertOnSubmit(adminBinding);
                    //                    database.SubmitChanges();
                    //                }
                    //                return new WechatTextMessage()
                    //                {
                    //                    Content = string.Format("You are registered as event admin for Event Group:{0}.",
                    //                    eventGroup.EventGroupName)
                    //                };
                    //            }
                    //            else
                    //            {
                    //                WechatLib.WriteLog("Bind event group admin: invalid event group id:" + arg.EventKey);
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        return new WechatTextMessage() { Content = "Welcome to Sample Sales." };
                    //    }
                    //}
                    #endregion
                    break;
                //case "CLICK":
                //    return new WechatTextMessage() { Content = "Service under maintenance." };
                case "SCANCODE_WAITMSG":
                    WechatScanMessage posMessage = WechatBaseMessage.FromXml<WechatScanMessage>(rawXml);
                    return new WechatTextMessage() { Content = posMessage.ScanCodeInfo.ScanType + "|" + posMessage.ScanCodeInfo.ScanResult };
            }
            return new WechatTextMessage();
        }

        protected WechatBaseMessage OnHandleTextMessage(Wechat.WechatTextMessage arg, string rawXml)
        {
            LogManager manager = new LogManager();
            SystemLog log = new SystemLog()
            {
                Type = "Log",
                Content = "WeChat Text Message Received. Message: " + rawXml,
                Time = DateTime.UtcNow
            };
            manager.AddLog(log);

            if (rawXml.Contains("扫码"))
            {
                return new WechatTextMessage()
                {
                    Content = "This is test for Scan bar code!"
                };
            }
            else if (rawXml.Contains("Name:"))
            {
                WeChatInboundMessage message = Util.ParseXMLMessage(rawXml);
                string content = message.Content;
                string name = content.Replace("Name:", "").Trim();
                string fromUerOpenId = message.FromUserName;

                UserAccountManager manage = new UserAccountManager();
                int result = manage.UpdateUserDisplayNameByOpenId(fromUerOpenId, name);
                if (result == 1)
                {
                    return new WechatTextMessage()
                    {
                        Content = string.Format("Got your name and updated it, {0}! Please reply your WeChat Id or Mobile Number with 'wxId:' before it, so that others like your posted item could contact you. || " +
                        "成功保存您的姓名，{1}！请以文字消息回复您的微信号或手机号，以便对您发布的物品感兴趣的人与您联系。注意加上英文前缀'wxId:'，如[wxId:12345678901]。", name, name)
                    };
                }
                else if (result == -1)
                {
                    return new WechatTextMessage()
                    {
                        Content = string.Format("Can not found your account in our system, {0}! || 系统找不到您的账号，{1}！", name, name)
                    };
                }
                else if (result == 0)
                {
                    return new WechatTextMessage()
                    {
                        Content = string.Format("Sorry {0}, failed to update your name, please contact support. || 抱歉，{1}，未能更新您的姓名，请联系系统支持！", name, name)
                    };
                }
            }
            else if (rawXml.Contains("wxId:"))
            {
                WeChatInboundMessage message = Util.ParseXMLMessage(rawXml);
                string content = message.Content;
                string wcNo = content.Replace("wxId:", "").Trim();
                string fromUerOpenId = message.FromUserName;

                UserAccountManager manage = new UserAccountManager();
                int result = manage.UpdateUserWeChatNumberByOpenId(fromUerOpenId, wcNo);
                if (result == 1)
                {
                    return new WechatTextMessage()
                    {
                        Content = string.Format("Got your WeChat Number and updated it, good to go! || 成功保存您的微信号/手机号，赶紧发布您的闲置物品吧！")
                    };
                }
                else if (result == -1)
                {
                    return new WechatTextMessage()
                    {
                        Content = string.Format("Can not found your account in our system! || 系统找不到您的账号！")
                    };
                }
                else if (result == 0)
                {
                    return new WechatTextMessage()
                    {
                        Content = string.Format("Sorry, failed to update your WeChat Number, please contact support. || 抱歉，未能更新您的微信号/手机号，请联系系统支持！")
                    };
                }
            }

            return new WechatTextMessage()
            {
                Content = "Can not understand, but if you have any question please contact support. Thank you!"
            };
        }

        protected WechatBaseMessage OnHandleImageMessage(Wechat.WechatImageMessage arg, string rawXml)
        {
            LogManager manager = new LogManager();
            SystemLog log = new SystemLog()
            {
                Type = "Log",
                Content = "WeChat Image Message Received.",
                Time = DateTime.UtcNow
            };
            manager.AddLog(log);
            return new WechatTextMessage() { Content = "Photo received." };

            //user upload badge photo
            //using (SampleSalesDatabaseDataContext database = new SampleSalesDatabaseDataContext())
            //{
            //    string uniqueName = database.GetUserUniqueNameByOpenId(arg.FromUserName);
            //    if (!string.IsNullOrWhiteSpace(uniqueName))
            //    {
            //        PhotoLogic.DownloadAndUpdateBadgePhoto(uniqueName, arg.PicUr);

            //        WechatLib.WriteLog(string.Format("User photo updated {0}|{1}.", uniqueName, arg.PicUr));

            //        //Participant participant = database.Participants.SingleOrDefault(p => p.UniqueName == uniqueName);
            //        //if (participant != null)
            //        //{
            //        //    participant.PhotoUrl = PhotoLogic.GetUserPhotoUrl(uniqueName);
            //        //    database.SubmitChanges();
            //        //}

            //        return new WechatTextMessage() { Content = "User photo updated." };
            //    }
            //    else
            //    {
            //        WechatLib.WriteLog(string.Format("User not identified. {0}|{1}", uniqueName, arg.FromUserName));
            //        return new WechatTextMessage() { Content = "Error: User not identified." };
            //    }
            //}
        }

        protected WechatBaseMessage OnHandleUnknownMessage(WechatBaseMessage arg, string rawXml)
        {
            using (Wechat.WechatMessageHandlerSvc.WechatSvcClient client = new Wechat.WechatMessageHandlerSvc.WechatSvcClient())
            {
                return client.HandleUnknownMessage(arg, rawXml);
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

            string jsToken = string.Empty;
            string AppId = ConfigurationManager.AppSettings["AppId"];
            string AppSecret = ConfigurationManager.AppSettings["AppSecret"];
            WeiXinPublicAccount wxpa = new WeiXinPublicAccount(AppId, AppSecret);

            WechatManager manager = new WechatManager();
            WechatCache cache = manager.GetWechatCache();
            if (cache == null)
            {
                int returnCode = wxpa.WeChatPublicAccount_OnAccessTokenExpired();
                if (returnCode == 1800)
                {
                    cache.Token = wxpa.AccessToken;
                    cache.Ticket = wxpa.jsapi_ticket;
                    cache.Timestamp = DateTime.Now;
                    manager.UpdateWechatCache(cache);

                    jsToken = wxpa.jsapi_ticket;
                }
            }

            string nonceStr = "Wm3WZYTPz0wzccnW";
            string timestamp = DateTime.Now.Ticks.ToString();
            string source = string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", jsToken, nonceStr, timestamp, currentURL.Split("#".ToArray(), StringSplitOptions.RemoveEmptyEntries)[0]);
            JsInitResponse result = new JsInitResponse()
            {
                appId = AppId,
                nonceStr = nonceStr,
                signature = WeChatCrypter.GenarateSignature(source),
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