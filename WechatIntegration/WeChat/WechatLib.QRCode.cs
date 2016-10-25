using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Wechat
{
    partial class WechatLib
    {
        internal const string GetQRCodeUrlTemplate = "https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token={0}";
        internal const string ShowQRCodeUrlTemplate = "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket={0}";
        internal const string DecryptCardCodeUrlTemplate = "https://api.weixin.qq.com/card/code/decrypt?access_token={0}";

        /// <summary>
        /// Get Wechat Card QRCode url.
        /// </summary>
        /// <param name="cardId">Wechat Card Id</param>
        /// <param name="cardCode">Custom Card Code, optional. 20 characters.</param>
        /// <param name="isUnique">Is code unique. If true then the code can only be scanned once.</param>
        /// <returns></returns>
        public static string GetCardQRCodeUrl(string cardId, int expireSeconds, string cardCode = null, bool isUnique = true)
        {
            QRCodeRequest qrCodeRequest = new QRCodeRequest();
            qrCodeRequest.ActionName = QRCodeRequest.ActionNames.QR_CARD.ToString();
            qrCodeRequest.ExpireSeconds = expireSeconds;
            qrCodeRequest.ActionInfo.CardObj = new QRCodeRequest.QRCodeActionInfo.Card()
            {
                CardCode = cardCode,
                CardId = cardId,
                IsUniqueCode = isUnique
            };

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(GetQRCodeUrlTemplate, AccessTokenData));
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            Stream stream = request.GetRequestStream();

            DataContractJsonSerializer serRequest = new DataContractJsonSerializer(typeof(QRCodeRequest));
            serRequest.WriteObject(stream, qrCodeRequest);
            stream.Close();

            HttpWebResponse res = (HttpWebResponse)request.GetResponse();
            string respondText = new StreamReader(res.GetResponseStream()).ReadToEnd();


            Error error = JsonTypedSerializer<Error>.Deserialize(respondText);
            if (error.ErrorCode != 0)
            {
                throw new InvalidOperationException(string.Format("Error: {0}|{1}", error.ErrorCode, error.ErrorMessage));
            }

            QRCodeResponse response = JsonTypedSerializer<QRCodeResponse>.Deserialize(respondText); ;
            return string.Format(ShowQRCodeUrlTemplate, response.Ticket);
        }

        /// <summary>
        /// Get Wechat Permanant QRCode url.
        /// </summary>
        /// <param name="sceneString">Wechat Scene String, which is the text code embedded in the QR graph.</param>
        /// <returns></returns>
        public static string GetPermanantQRCodeUrl(string sceneString)
        {
            QRCodeRequest qrCodeRequest = new QRCodeRequest();
            qrCodeRequest.ActionName = QRCodeRequest.ActionNames.QR_LIMIT_STR_SCENE.ToString();
            qrCodeRequest.ExpireSeconds = 0;
            qrCodeRequest.ActionInfo.Scene = new QRCodeRequest.QRCodeActionInfo.QRCodeScene()
            {
                SceneString = sceneString
            };

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(GetQRCodeUrlTemplate, AccessTokenData));
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            Stream stream = request.GetRequestStream();

            DataContractJsonSerializer serRequest = new DataContractJsonSerializer(typeof(QRCodeRequest));
            serRequest.WriteObject(stream, qrCodeRequest);
            stream.Close();

            HttpWebResponse res = (HttpWebResponse)request.GetResponse();
            string respondText = new StreamReader(res.GetResponseStream()).ReadToEnd();


            Error error = JsonTypedSerializer<Error>.Deserialize(respondText);
            if (error.ErrorCode != 0)
            {
                throw new InvalidOperationException(string.Format("Error: {0}|{1}", error.ErrorCode, error.ErrorMessage));
            }

            QRCodeResponse response = JsonTypedSerializer<QRCodeResponse>.Deserialize(respondText); ;
            return string.Format(ShowQRCodeUrlTemplate, response.Ticket);
        }

        /// <summary>
        /// Get Wechat temporary QRCode URL. Maximum duration 30 days.
        /// </summary>
        /// <param name="sceneId">Wechat scene id. Between 1 and 100000.</param>
        /// <returns>QRCode image url</returns>
        public static string GetTemporaryQRCodeUrl(int sceneId, int expireSeconds = 2592000)
        {
            QRCodeRequest qrCodeRequest = new QRCodeRequest();
            qrCodeRequest.ActionName = QRCodeRequest.ActionNames.QR_SCENE.ToString();
            qrCodeRequest.ExpireSeconds = expireSeconds;
            qrCodeRequest.ActionInfo.Scene = new QRCodeRequest.QRCodeActionInfo.QRCodeScene()
            {
                SceneId = sceneId
            };

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(GetQRCodeUrlTemplate, AccessTokenData));
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            Stream stream = request.GetRequestStream();

            DataContractJsonSerializer serRequest = new DataContractJsonSerializer(typeof(QRCodeRequest));
            serRequest.WriteObject(stream, qrCodeRequest);
            stream.Close();

            HttpWebResponse res = (HttpWebResponse)request.GetResponse();
            string respondText = new StreamReader(res.GetResponseStream()).ReadToEnd();


            Error error = JsonTypedSerializer<Error>.Deserialize(respondText);
            if (error.ErrorCode != 0)
            {
                throw new InvalidOperationException(string.Format("Error: {0}|{1}", error.ErrorCode, error.ErrorMessage));
            }

            QRCodeResponse response = JsonTypedSerializer<QRCodeResponse>.Deserialize(respondText); ;
            return string.Format(ShowQRCodeUrlTemplate, response.Ticket);
        }

        /// <summary>
        /// Get Wechat QRCode image data in binary format.
        /// </summary>
        /// <param name="qrcodeUrl">QRCode URL returned by Wechat service</param>
        /// <returns></returns>
        public static byte[] GetQRCodeData(string qrcodeUrl)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(qrcodeUrl);
            request.Method = "GET";
            request.ContentType = "application/json; charset=utf-8";
            HttpWebResponse res = (HttpWebResponse)request.GetResponse();
            MemoryStream memStream = new MemoryStream();
            res.GetResponseStream().CopyTo(memStream);
            return memStream.ToArray();
        }

        public static string DecryptCardCode(string encryptedCardCode)
        {
            DecryptCodeRequest decryptRequest = new DecryptCodeRequest();
            decryptRequest.EncryptedCode = encryptedCardCode;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(DecryptCardCodeUrlTemplate, AccessTokenData));
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            Stream stream = request.GetRequestStream();

            DataContractJsonSerializer serRequest = new DataContractJsonSerializer(typeof(DecryptCodeRequest));
            serRequest.WriteObject(stream, decryptRequest);
            stream.Close();

            HttpWebResponse res = (HttpWebResponse)request.GetResponse();
            string respondText = new StreamReader(res.GetResponseStream()).ReadToEnd();

            DecryptCodeResponse response = JsonTypedSerializer<DecryptCodeResponse>.Deserialize(respondText); ;

            if (response.ErrorCode != 0)
            {
                throw new InvalidOperationException(string.Format("Error: {0}|{1}", response.ErrorCode, response.ErrorMessage));
            }
            return response.Code;
        }
    }

    #region QRCodeRequest
    [DataContract]
    public class QRCodeRequest
    {
        public enum ActionNames
        {
            QR_SCENE,
            QR_LIMIT_STR_SCENE,
            QR_LIMIT_SCENE,
            QR_CARD
        }

        [DataMember(Name = "expire_seconds", EmitDefaultValue = true)]
        public int ExpireSeconds;

        [DataMember(Name = "action_name", EmitDefaultValue = true)]
        public string ActionName;

        [DataMember(Name = "action_info", EmitDefaultValue = true)]
        public QRCodeActionInfo ActionInfo = new QRCodeActionInfo();

        [DataContract]
        public class QRCodeActionInfo
        {
            [DataMember(Name = "scene", EmitDefaultValue = true)]
            public QRCodeScene Scene = null;

            [DataMember(Name = "card", EmitDefaultValue = true)]
            public Card CardObj = null;

            [DataContract]
            public class QRCodeScene
            {
                [DataMember(Name = "scene_id", EmitDefaultValue = true)]
                public int? SceneId = null;

                [DataMember(Name = "scene_str", EmitDefaultValue = true)]
                public string SceneString = null;
            }

            [DataContract]
            public class Card
            {
                [DataMember(Name = "card_id", EmitDefaultValue = true)]
                public string CardId = null;

                [DataMember(Name = "code", EmitDefaultValue = true)]
                public string CardCode = null;

                [DataMember(Name = "openid", EmitDefaultValue = true)]
                public int? OpenId = null;

                [DataMember(Name = "is_unique_code", EmitDefaultValue = true)]
                public bool IsUniqueCode;

                [DataMember(Name = "outer_id", EmitDefaultValue = true)]
                public int? OuterId = null;
            }
        }
    }
    #endregion

    #region QRCodeResponse
    [DataContract]
    public class QRCodeResponse
    {
        [DataMember(Name = "expire_seconds", EmitDefaultValue = true)]
        public int ExpireSeconds;

        [DataMember(Name = "ticket", EmitDefaultValue = true)]
        public string Ticket;

        [DataMember(Name = "url", EmitDefaultValue = true)]
        public string Url;
    }
    #endregion

    #region DecryptCodeRequest
    [DataContract]
    public class DecryptCodeRequest
    {
        [DataMember(Name = "encrypt_code", EmitDefaultValue = true)]
        public string EncryptedCode;
    }
    #endregion

    #region DecryptCodeResponse
    [DataContract]
    public class DecryptCodeResponse
    {
        [DataMember(Name = "errcode", EmitDefaultValue = true)]
        public int ErrorCode;

        [DataMember(Name = "errmsg", EmitDefaultValue = true)]
        public string ErrorMessage;

        [DataMember(Name = "code", EmitDefaultValue = true)]
        public string Code;
    }
    #endregion

    #region WechatCardEventMessage
    [XmlRoot("xml")]
    public class WechatCardEventMessage : WechatEventMessage
    {
        [XmlElement("CardId")]
        public XmlCDataSection CardIdCData { get; set; }

        [XmlIgnore]
        public string CardId
        {
            get
            {
                return CardIdCData.Value;
            }
            set
            {
                CardIdCData.Value = value;
            }
        }

        [XmlElement("UserCardCode")]
        public XmlCDataSection UserCardCodeCData { get; set; }

        [XmlIgnore]
        public string UserCardCode
        {
            get
            {
                return UserCardCodeCData.Value;
            }
            set
            {
                UserCardCodeCData.Value = value;
            }
        }

        [XmlElement("OuterId")]
        public int OuterId { get; set; }

        [XmlElement("IsGiveByFriend")]
        public int IsGiveByFriend { get; set; }

        [XmlElement("FriendUserName")]
        public XmlCDataSection FriendUserNameCData { get; set; }

        [XmlIgnore]
        public string FriendUserName
        {
            get
            {
                return FriendUserNameCData.Value;
            }
            set
            {
                FriendUserNameCData.Value = value;
            }
        }
    }
    #endregion
}
