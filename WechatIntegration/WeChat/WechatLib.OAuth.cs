using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Wechat
{
    public partial class WechatLib
    {
        internal const string OAuthUrlTemplate = "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope={2}&state={3}#wechat_redirect";
        internal const string OAuthGetAccessTokenUrlTemplate = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";

        public enum OAuthScope
        {
            snsapi_base,
            snsapi_userinfo
        }

        public static string GetOAuthUrl(string appId, string redirectUriHTTPS, OAuthScope scope, string state)
        {
            string url = string.Format(OAuthUrlTemplate, appId, HttpUtility.UrlEncode(redirectUriHTTPS), scope.ToString(), state);
            WechatLib.WriteLog("OAuthUrl converted:" + url);
            return url;
        }

        /// <summary>
        /// Get OpenId by encrypted code. The code is sent by Wechat within each OAuth request.
        /// </summary>
        /// <param name="appId">Wechat AppId</param>
        /// <param name="appSecret">Wechat AppSecret</param>
        /// <param name="code">OAuth encrypted code</param>
        /// <returns>OAuthAccess token containing the openid</returns>
        internal static OAuthAccessToken GetOAuthOpenIdByCode(string appId, string appSecret, string code)
        {
            //send code to OAuthGetAccessTokenUrl
            string url = string.Format(OAuthGetAccessTokenUrlTemplate, appId, appSecret, code);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            WechatLib.WriteLog("OAuth get access token:" + url);
            OAuthAccessToken accessToken = null;
            try
            {
                HttpWebResponse res = (HttpWebResponse)request.GetResponse();
                string respondText = new StreamReader(res.GetResponseStream()).ReadToEnd();
                WechatLib.WriteLog("OAuth access token received:" + respondText);

                //retrieve json and return OAuthAccessToken
                accessToken = JsonTypedSerializer<OAuthAccessToken>.Deserialize(respondText);
                if (accessToken != null)
                {
                    return accessToken;
                }
                else
                {
                    Error error = JsonTypedSerializer<Error>.Deserialize(respondText);
                    throw new InvalidOperationException(string.Format("Error: {0}|{1}", error.ErrorCode, error.ErrorMessage));
                }
            }
            catch (Exception ex)
            {
                WechatLib.WriteLog("Get OAuth access token exception:" + ex.ToString());
                return accessToken;
            }
        }

        /// <summary>
        /// Gets OpenId from cookie with the given http context.
        /// </summary>
        /// <returns>Wechat OpenId text</returns>
        /// <param name="context">HttpContext to get the cookie</param>
        public static string GetOpenId(HttpContext context)
        {
            var cookieKey = ConfigurationManager.AppSettings["OpenIDKey"];
            HttpCookie cookie;
            if (cookieKey == null)
            {
                cookie = context.Request.Cookies.Get("OpenId");
            }
            else
            {
                cookie = context.Request.Cookies.Get(cookieKey);
            }
            if (cookie != null)
                return cookie.Value;
            else
                return null;
        }

        /// <summary>
        /// Sets OpenId to cookie with the given http context.
        /// </summary>
        /// <param name="value">OpenId text to save</param>
        /// <param name="context">HttpContext to set the cookie</param>
        internal static void SetOpenId(HttpContext context, string value)
        {
            var cookieKey = ConfigurationManager.AppSettings["OpenIDKey"];
            if (cookieKey == null)
            {
                context.Response.SetCookie(new HttpCookie("OpenId", value));
            }
            else
            {
                context.Response.SetCookie(new HttpCookie(cookieKey, value));
            }
        }

        #region OAuthAccessToken
        [DataContract]
        public class OAuthAccessToken
        {
            [DataMember(Name = "access_token", EmitDefaultValue = true)]
            public string AccessToken;

            [DataMember(Name = "expires_in", EmitDefaultValue = true)]
            public int ExpireSeconds;

            [DataMember(Name = "refresh_token", EmitDefaultValue = true)]
            public string RefreshToken;

            [DataMember(Name = "openid", EmitDefaultValue = true)]
            public string OpenId;

            [DataMember(Name = "scope", EmitDefaultValue = true)]
            public string Scope;
        }
        #endregion
    }
}
