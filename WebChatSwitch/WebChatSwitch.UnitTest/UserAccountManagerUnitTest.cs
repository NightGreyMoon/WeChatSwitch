using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebChatSwitch.BLL;
using System.Configuration;
using WeChatPublicAccount;
using WebChatSwitch.DAL;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace WebChatSwitch.UnitTest
{
    [TestClass]
    public class UserAccountManagerUnitTest
    {
        [TestMethod]
        public void BasicDataAccessTest()
        {
            UserAccountManager manager = new UserAccountManager();
            var account = manager.GetUserAccountInfoByOpenId("haobohaobohaobohaobohaobo123");
            Assert.IsNotNull(account);
            Assert.AreNotEqual(0, account.Items.Count);
        }

        [TestMethod]
        public void GetSubscriberTest()
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

            var response = HttpClient.Get(string.Format("https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}", accessToken, "oZgK_wQsus9-MdIOmEbi14hWi1js"));
            var stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(response.GetResponseStream());
            string responseContent = streamReader.ReadToEnd();
            response.Close();
            streamReader.Close();

            JObject jo = JObject.Parse(responseContent);
            string nickname = jo.Properties().FirstOrDefault(pr => pr.Name == "nickname").Value.ToString();
            string language = jo.Properties().FirstOrDefault(pr => pr.Name == "language").Value.ToString();
            string openid = jo.Properties().FirstOrDefault(pr => pr.Name == "openid").Value.ToString();
            string city = jo.Properties().FirstOrDefault(pr => pr.Name == "city").Value.ToString();
            string province = jo.Properties().FirstOrDefault(pr => pr.Name == "province").Value.ToString();
            Assert.IsNotNull(jo);
        }
    }
}
