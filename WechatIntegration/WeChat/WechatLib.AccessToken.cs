using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using WechatEntities;

namespace Wechat
{
    public partial class WechatLib
    {
        internal const string GetAccessTokenUrlTemplate = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";

        /// <summary>
        /// Get Wechat Access Token. The token expires in 7200 seconds. If the call fails, throw InvalidOperationException.
        /// </summary>
        /// <returns>Access Token object</returns>
        public static WechatAccessToken GetAccessToken()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(GetAccessTokenUrlTemplate, AppId, AppSecret));
            request.Method = "GET";
            request.ContentType = "content-type: text/html; charset=utf-8";

            HttpWebResponse res = (HttpWebResponse)request.GetResponse();
            Stream resStream = res.GetResponseStream();

            DataContractJsonSerializer serAccessToken = new DataContractJsonSerializer(typeof(WechatAccessToken));
            WechatAccessToken token = (WechatAccessToken)serAccessToken.ReadObject(resStream);
            if (token == null || token.AccessTokenData == null)
            {
                DataContractJsonSerializer serError = new DataContractJsonSerializer(typeof(Error));
                Error error = (Error)serError.ReadObject(resStream);
                throw new InvalidOperationException(string.Format("Error: {0}|{1}", error.ErrorCode, error.ErrorMessage));
            }
            else
            {
                return token;
            }

        }

        /// <summary>
        /// Cache Access Token to wechat database
        /// </summary>
        /// <param name="token">Access Token to be saved</param>
        internal static void SaveAccessTokenToDatabaseCache(WechatAccessToken token)
        {
            using (WechatDBDataContext database = new WechatDBDataContext())
            {
                AccessToken accessTokenEntity = new AccessToken();
                accessTokenEntity.AccessTokenData = token.AccessTokenData;
                accessTokenEntity.ExpireBy = DateTime.UtcNow.AddSeconds(AccessTokenSafeExpire);
                database.AccessTokens.InsertOnSubmit(accessTokenEntity);
                database.SubmitChanges();
            }
        }

        /// <summary>
        /// Load Cached Access Token from wechat database
        /// </summary>
        /// <param name="token">Cached Access Token</param>
        internal static string LoadAccessTokenFromDatabaseCache()
        {
            using (WechatDBDataContext database = new WechatDBDataContext())
            {
                AccessToken token = database.AccessTokens.OrderByDescending(t => t.ExpireBy).FirstOrDefault();
                if (token == null)
                {
                    return null;
                }
                else
                {
                    if (token.ExpireBy <= DateTime.UtcNow)
                    {
                        return null;
                    }
                    return token.AccessTokenData;
                }
            }
        }

        public static void ClearAccessTokenCache()
        {
            using (WechatDBDataContext database = new WechatDBDataContext())
            {
                database.AccessTokens.DeleteAllOnSubmit(database.AccessTokens);
                database.SubmitChanges();
            }
        }
    }

    [DataContract]
    public class WechatAccessToken
    {
        [DataMember(Name = "access_token")]
        public string AccessTokenData;

        [DataMember(Name = "expires_in")]
        public int ExpiresIn;
    }
}
