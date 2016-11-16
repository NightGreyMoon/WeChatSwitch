using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebChatSwitch.BLL;
using WebChatSwitch.DAL;
using WeChatPublicAccount;

namespace WebChatSwitch.Web.Controllers
{
    public class WeChatBaseController : Controller
    {
        public LoginUser CurrentUser
        {
            get
            {
                if (Session["LoginUser"] != null)
                {
                    return Session["LoginUser"] as LoginUser;
                }
                return null;
            }

            set
            {
                Session["LoginUser"] = value;
            }
        }

        public class LoginUser
        {
            public string OpenId { get; set; }
            public int Id { get; set; }
        }

        public JsInitResponse InitialWechatSDK(string currentURL)
        {

            string jsToken = string.Empty;
            string AppId = ConfigurationManager.AppSettings["AppId"];
            string AppSecret = ConfigurationManager.AppSettings["AppSecret"];
            WeiXinPublicAccount wxpa = new WeiXinPublicAccount(AppId, AppSecret);

            WechatManager manager = new WechatManager();
            WechatCache cache = manager.GetWechatCache();
            if (cache == null)
            {
                cache = new WechatCache();
                int returnCode = wxpa.WeChatPublicAccount_OnAccessTokenExpired();
                if (returnCode == 1800)
                {
                    cache.Token = wxpa.AccessToken;
                    cache.Ticket = wxpa.jsapi_ticket;
                    cache.Timestamp = DateTime.UtcNow;
                    manager.UpdateWechatCache(cache);
                }
            }
            jsToken = cache.Ticket;

            string nonceStr = "Wm3WZYTPz0wzccnW";
            string timestamp = Util.CreateTimestamp().ToString();


            string source = string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", jsToken, nonceStr, timestamp, currentURL);
            //string signature = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(source, "SHA1");

            //string source = "jsapi_ticket=kgt8ON7yVITDhtdwci0qeTQIB0ds82IxTu3pDvW_gUqD60Q9zCrv1YkvovovbWdlJzFtIPryUYqDTrPQ1Sk6ww&noncestr=Wm3WZYTPz0wzccnW&timestamp=1414587457&url=http://webchatswitch.azurewebsites.net/Item/Create";

            //建立SHA1对象
            SHA1 sha = new SHA1CryptoServiceProvider();
            //将mystr转换成byte[] 
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] dataToHash = enc.GetBytes(source);
            //Hash运算
            byte[] dataHashed = sha.ComputeHash(dataToHash);
            //将运算结果转换成string
            string signature = BitConverter.ToString(dataHashed).Replace("-", "").ToLower();



            JsInitResponse result = new JsInitResponse()
            {
                appId = AppId,
                nonceStr = nonceStr,
                signature = signature,
                timestamp = timestamp
            };
            //string source = string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", jsToken, nonceStr, timestamp, currentURL.Split("#".ToArray(), StringSplitOptions.RemoveEmptyEntries)[0]);
            //JsInitResponse result = new JsInitResponse()
            //{
            //    appId = AppId,
            //    nonceStr = nonceStr,
            //    signature = WeChatCrypter.GenarateSignature(source),
            //    timestamp = timestamp
            //};
            return result;
        }

        public string GetAccessToken()
        {
            string accessToken = string.Empty;
            string AppId = ConfigurationManager.AppSettings["AppId"];
            string AppSecret = ConfigurationManager.AppSettings["AppSecret"];
            WeiXinPublicAccount wxpa = new WeiXinPublicAccount(AppId, AppSecret);

            WechatManager manager = new WechatManager();
            WechatCache cache = manager.GetWechatCache();
            if (cache == null)
            {
                cache = new WechatCache();
                int returnCode = wxpa.WeChatPublicAccount_OnAccessTokenExpired();
                if (returnCode == 1800)
                {
                    cache.Token = wxpa.AccessToken;
                    cache.Ticket = wxpa.jsapi_ticket;
                    cache.Timestamp = DateTime.UtcNow;
                    manager.UpdateWechatCache(cache);
                }
            }
            accessToken = cache.Token;
            return accessToken;
        }

        //获取临时素材
        //https://api.weixin.qq.com/cgi-bin/media/get?access_token=ACCESS_TOKEN&media_id=MEDIA_ID 

        //微信下载图片
        public string WeChatDownloadImage(string mediaId)
        {
            string accessToken = GetAccessToken();

            LogManager logManager = new LogManager();
            SystemLog log = new SystemLog()
            {
                Type = "Log",
                Content = "Token got to get the temp images, Token: " + accessToken,
                Time = DateTime.UtcNow
            };
            logManager.AddLog(log);

            string result = string.Empty;
            try
            {
                var response = WeChatPublicAccount.HttpClient.Get(string.Format("https://api.weixin.qq.com/cgi-bin/media/get?access_token={0}&media_id={1}", accessToken, mediaId));
                var stream = response.GetResponseStream();

                byte[] b = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        byte[] buf = new byte[1024];
                        count = stream.Read(buf, 0, 1024);
                        ms.Write(buf, 0, count);
                    } while (stream.CanRead && count > 0);
                    b = ms.ToArray();
                }
                stream.Close();

                string filename = DateTime.Now.Ticks + ".jpg";
                string ftpurl = "ftp://waws-prod-hk1-003.ftp.azurewebsites.windows.net/site/wwwroot/UploadedPic/" + filename;
                string ftpusername = "scrumoffice\\Haobo";
                string ftppassword = "DHB482dhb";

                FtpWebRequest ftpClient = (FtpWebRequest)FtpWebRequest.Create(ftpurl);
                ftpClient.Credentials = new System.Net.NetworkCredential(ftpusername, ftppassword);
                ftpClient.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
                ftpClient.UseBinary = true;
                ftpClient.KeepAlive = true;
                System.IO.Stream rs = ftpClient.GetRequestStream();
                rs.Write(b, 0, b.Length);
                ftpClient.ContentLength = b.Length;
                rs.Close();
                FtpWebResponse uploadResponse = (FtpWebResponse)ftpClient.GetResponse();
                string value = uploadResponse.StatusDescription;
                uploadResponse.Close();

                SystemLog saveLog = new SystemLog()
                {
                    Type = "Log",
                    Content = "Return response for saving image via ftp: " + value,
                    Time = DateTime.UtcNow
                };
                logManager.AddLog(saveLog);

                //Bitmap bitmap = new Bitmap(stream);
                string basePath = "/UploadedPic/";
                //string fullName = Server.MapPath(basePath) + filename;

                //SystemLog fileNameLog = new SystemLog()
                //{
                //    Type = "Log",
                //    Content = "Path to save image: " + fullName,
                //    Time = DateTime.UtcNow
                //};
                //logManager.AddLog(fileNameLog);    

                //Bitmap bm2 = new Bitmap(bitmap.Width, bitmap.Height);
                //Graphics g = Graphics.FromImage(bm2);
                //g.DrawImageUnscaled(bitmap, 0, 0);
                //bm2.Save(fullName);

                //返回地址
                string path = @"ftp://scrumoffice%5CHaobo:DHB482dhb@waws-prod-hk1-003.ftp.azurewebsites.windows.net/site/wwwroot";
                result = path + basePath + filename;
            }
            catch (Exception ex)
            {
                SystemLog exLog = new SystemLog()
                {
                    Type = "Exception",
                    Content = ex.Message,
                    Time = DateTime.UtcNow
                };
                logManager.AddLog(exLog);
            }
            return result;
        }

        public bool WeChatGetUserBasicInfoByOpenId(string openId)
        {
            bool inserted = false;
            LogManager logManager = new LogManager();
            try
            {
                string accessToken = GetAccessToken();
                SystemLog log = new SystemLog()
                {
                    Type = "Log",
                    Content = "Token got to get the base information for subscriber, Token: " + accessToken,
                    Time = DateTime.UtcNow
                };
                logManager.AddLog(log);

                var response = WeChatPublicAccount.HttpClient.Get(string.Format("https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}", accessToken, openId));
                var stream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(response.GetResponseStream());
                string responseContent = streamReader.ReadToEnd();
                response.Close();
                streamReader.Close();

                SystemLog reLog = new SystemLog()
                {
                    Type = "Log",
                    Content = "Got the response for base information of new subscriber, Response Content: " + responseContent,
                    Time = DateTime.UtcNow
                };
                logManager.AddLog(reLog);

                JObject jo = JObject.Parse(responseContent);
                string nickname = jo.Properties().FirstOrDefault(pr => pr.Name == "nickname").Value.ToString();
                string language = jo.Properties().FirstOrDefault(pr => pr.Name == "language").Value.ToString();
                string city = jo.Properties().FirstOrDefault(pr => pr.Name == "city").Value.ToString();
                string province = jo.Properties().FirstOrDefault(pr => pr.Name == "province").Value.ToString();

                UserAccount ua = new UserAccount()
                {
                    WeChatNickName = nickname,
                    OpenId = openId,
                    Balance = 0
                };

                SystemLog uaLog = new SystemLog()
                {
                    Type = "Log",
                    Content = "Got the base information for subscriber, openId: " + openId + "; nickname: " + nickname,
                    Time = DateTime.UtcNow
                };
                logManager.AddLog(uaLog);

                UserAccountManager uaManager = new UserAccountManager();
                inserted = uaManager.InsertUserAccountByOpenId(ua);
            }
            catch (Exception ex)
            {
                SystemLog log = new SystemLog()
                {
                    Type = "Exception",
                    Content = "Exception occured to create account for new subscriber, Message: " + ex.Message,
                    Time = DateTime.UtcNow
                };
                logManager.AddLog(log);
            }
            return inserted;
        }

        #region override
        //protected override void OnException(ExceptionContext filterContext)
        //{
        //    base.OnException(filterContext);

        //    var ex = filterContext.Exception;
        //    while (ex != null)
        //    {
        //        log.Error("程序异常", filterContext.Exception);
        //        ex = ex.InnerException;
        //    }

        //    filterContext.ExceptionHandled = true;

        //    if (CurrentUser.Language == "en")
        //    {
        //        TempData["Info"] = "System Error";
        //    }
        //    else
        //    {
        //        TempData["Info"] = "系统异常";
        //    }
        //    filterContext.Result = new RedirectResult("~/Error/Index");

        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        if (db != null)
        //        {
        //            db.Dispose();
        //        }
        //    }
        //    base.Dispose(disposing);
        //}

        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    base.OnActionExecuting(filterContext);

        //    var lan = Request.QueryString["lan"];

        //    if (CurrentUser == null)
        //    {
        //        CurrentUser = new LoginUser { Language = lan };
        //    }

        //    #region set culture
        //    string cultureName = null;
        //    if (!string.IsNullOrWhiteSpace(lan))
        //    {
        //        cultureName = lan;
        //    }
        //    else if (CurrentUser != null && !string.IsNullOrWhiteSpace(CurrentUser.Language))
        //        cultureName = CurrentUser.Language;
        //    else
        //    {
        //        cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ?
        //                Request.UserLanguages[0] :  // obtain it from HTTP header AcceptLanguages
        //                null;

        //    }

        //    // Validate culture name
        //    cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

        //    // Modify current thread's cultures            
        //    Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
        //    Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
        //    CurrentUser.Language = cultureName;
        //    #endregion
        //}

        //protected override void OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //    base.OnActionExecuted(filterContext);

        //    ViewBag.Language = CurrentUser.Language;
        //}

        #endregion
    }
}