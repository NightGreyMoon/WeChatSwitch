using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Wechat
{
    public class WechatOAuthBasePage : System.Web.UI.Page
    {
        internal string Code { get { return Request.QueryString["code"]; } }
        public string WechatState { get { return Request.QueryString["state"]; } }

        /// <summary>
        /// Get the Wechat OpenId
        /// </summary>
        public static string WechatOpenId
        {
            get
            {
                return WechatLib.GetOpenId(HttpContext.Current);                
            }
            private set
            {
                WechatLib.SetOpenId(HttpContext.Current, value);
            }
        }

        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);
            if (WechatOpenId == null)
                HandleOAuthRequest();
        }

        /// <summary>
        /// Process OAuth request and parse the open id.
        /// </summary>
        private void HandleOAuthRequest()
        {
            try
            {
                WechatLib.OAuthAccessToken token = WechatLib.GetOAuthOpenIdByCode(WechatLib.AppId, WechatLib.AppSecret, Code);
                WechatOpenId = token.OpenId;
                WechatLib.WriteLog("OAuth success. OpenId=" + WechatOpenId);
            }
            catch (Exception e)
            {
                WechatOpenId = string.Empty;
                WechatLib.WriteLog("OAuth failed. " + e.ToString());
            }
        }
    }
}