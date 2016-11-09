using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Wechat
{
    partial class WechatLib
    {
        internal const string UploadMediaUrlTemplate = "http://file.api.weixin.qq.com/cgi-bin/media/upload?access_token={0}&type={1}";

        public enum MediaType
        {
            image,
            voice,
            video,
            thumb
        }

        public static string UploadMedia(Stream mediaData, MediaType type)
        {
            BinaryReader br = new BinaryReader(mediaData);
            return UploadMedia(br.ReadBytes(0), type);
        }

        public static string UploadMedia(byte[] mediaData, MediaType type)
        {

            if (mediaData != null)
            {
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(UploadMediaUrlTemplate, AccessTokenData, type.ToString()));
                //request.Method = "POST";
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentLength = mediaData.Length;
                //Stream stream = request.GetRequestStream();
                //stream.Write(mediaData, 0, mediaData.Length);

                //stream.Close();
                WebClient client = new WebClient();
                byte[] response = client.UploadData(string.Format(UploadMediaUrlTemplate, AccessTokenData, type.ToString()), mediaData);
                //HttpWebResponse res = (HttpWebResponse)request.GetResponse();
                // Stream resStream = res.GetResponseStream();
                return Encoding.Default.GetString(response);
                //DataContractJsonSerializer serError = new DataContractJsonSerializer(typeof(Error));
                //Error error = (Error)serError.ReadObject(resStream);
                //if (error.ErrorCode != 0)
                //{
                //    throw new InvalidOperationException(string.Format("Error: {0}|{1}", error.ErrorCode, error.ErrorMessage));
                //}
            }
            return "";

        }
    }

    public class WechatMedia
    {

    }
}
