using System;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Xml;
using System.Xml.Serialization;
using Wechat.Properties;
using WechatEntities;

namespace Wechat
{
    public partial class WechatLib
    {
        public enum MessageDirection
        {
            Inbound,
            Outbound
        }

        /// <summary>
        /// AppID
        /// </summary>
        public static string AppId
        {
            get
            {
                return ConfigurationManager.AppSettings["AppID"];
            }
        }

        /// <summary>
        /// AppSecret
        /// </summary>
        public static string AppSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["AppSecret"];
            }
        }

        /// <summary>
        /// Wechat AccessToken expires in 7200 seconds. 
        /// The web server cache should expire before that, so as to get a new token from Wechat.
        /// </summary>
        private const int AccessTokenSafeExpire = 3600;

        /// <summary>
        /// Gets AccessToken content from Cache. 
        /// If cache expired then call Wechat API to get a new token and register to web cache.
        /// </summary>
        public static string AccessTokenData
        {
            get
            {
                //try load cached access token from database
                string token = LoadAccessTokenFromDatabaseCache();
                if (token == null)
                {
                    //Get new token and save to cache db.
                    SaveAccessTokenToDatabaseCache(GetAccessToken());
                    token = LoadAccessTokenFromDatabaseCache();
                }
                return token;
            }
        }

        internal static void WriteLog(string logContent, DateTime logTime)
        {
            using (WechatDBDataContext db = new WechatDBDataContext(Settings.Default.WechatDBConnectionString))
            {
                WechatLog log = new WechatLog();
                log.LogContent = logContent;
                log.LogTime = logTime;
                db.WechatLogs.InsertOnSubmit(log);
                db.SubmitChanges();
            }
        }

        public static void WriteLog(string logContent)
        {
            WriteLog(logContent, DateTime.UtcNow);
        }

        public static void WriteMessageLog(WechatBaseMessage message, MessageDirection direction, string rawData)
        {
            using (WechatDBDataContext db = new WechatDBDataContext(Settings.Default.WechatDBConnectionString))
            {
                WechatMessage data = new WechatMessage();
                data.CreateTime = message.CreateTime;
                data.FromUserName = message.FromUserName;
                data.MsgType = message.MsgType;
                data.Direction = direction.ToString();
                data.ToUserName = message.ToUserName;
                data.RawData = rawData;
                db.WechatMessages.InsertOnSubmit(data);
                db.SubmitChanges();
            }
        }

        #region Signature and URL verification
        /// <summary>
        /// Signature Token is for Wechat to verify custom server security.
        /// </summary>
        public static string SignatureToken
        {
            get
            {
                return ConfigurationManager.AppSettings["SignatureToken"];
            }
        }

        public static bool CheckSignature(string signature, string timestamp, string nonce)
        {
            string[] strs = new string[] { SignatureToken, timestamp, nonce };
            Array.Sort(strs);
            string strNew = string.Join("", strs);
            strNew = FormsAuthentication.HashPasswordForStoringInConfigFile(strNew, "SHA1");
            if (signature == strNew.ToLower())
            {
                return true;
            }
            else { return false; }
        }
        #endregion
    }


}