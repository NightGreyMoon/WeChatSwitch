using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Wechat
{
    public partial class WechatLib
    {
        #region Sample menu request
        //{
        //     "button":[
        //     {	
        //          "type":"click",
        //          "name":"今日歌曲",
        //          "key":"V1001_TODAY_MUSIC"
        //      },
        //      {
        //           "type":"click",
        //           "name":"歌手简介",
        //           "key":"V1001_TODAY_SINGER"
        //      },
        //      {
        //           "name":"菜单",
        //           "sub_button":[
        //           {	
        //               "type":"view",
        //               "name":"搜索",
        //               "url":"http://www.soso.com/"
        //            },
        //            {
        //               "type":"view",
        //               "name":"视频",
        //               "url":"http://v.qq.com/"
        //            },
        //            {
        //               "type":"click",
        //               "name":"赞一下我们",
        //               "key":"V1001_GOOD"
        //            }]
        //       }]
        // }
        #endregion

        internal const string CreateMenuUrlTemplate = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token={0}";

        public static void CreateMenu(WechatMenu menu)
        {
            if (menu != null)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(CreateMenuUrlTemplate, AccessTokenData));
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";
                Stream stream = request.GetRequestStream();

                DataContractJsonSerializer serMenu = new DataContractJsonSerializer(typeof(WechatMenu));
                serMenu.WriteObject(stream, menu);
                stream.Close();

                HttpWebResponse res = (HttpWebResponse)request.GetResponse();
                Stream resStream = res.GetResponseStream();

                DataContractJsonSerializer serError = new DataContractJsonSerializer(typeof(Error));
                Error error = (Error)serError.ReadObject(resStream);
                if (error.ErrorCode != 0)
                {
                    throw new InvalidOperationException(string.Format("Error: {0}|{1}", error.ErrorCode, error.ErrorMessage));
                }
            }
        }
    }

    [DataContract]
    public class WechatMenu
    {
        [DataMember(Name = "button", EmitDefaultValue = true)]
        public WechatMenuButton[] Buttons;
    }

    [DataContract]
    public class WechatMenuButton
    {
        [DataMember(Name = "type", EmitDefaultValue = true)]
        public string Type = null;

        [DataMember(Name = "name", EmitDefaultValue = true)]
        public string Name = null;

        [DataMember(Name = "url", EmitDefaultValue = false, IsRequired = false)]
        public string Url;

        [DataMember(Name = "key", EmitDefaultValue = false, IsRequired = false)]
        public string Key;

        [DataMember(Name = "sub_button", EmitDefaultValue = false, IsRequired = false)]
        public WechatMenuButton[] SecondLevelMenuItems;
    }
}
