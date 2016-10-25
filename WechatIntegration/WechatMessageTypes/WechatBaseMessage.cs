using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Wechat
{

    public class XmlTypedSerializer<T>
    {
        public static string Serialize<U>(U obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            var xmlnsEmpty = new XmlSerializerNamespaces();
            xmlnsEmpty.Add("", "");
            StringWriter sw = new StringWriter();
            XmlWriter writer = XmlWriter.Create(sw, new XmlWriterSettings()
            {
                OmitXmlDeclaration = true,
                Indent = true
            }
            );
            serializer.Serialize(writer, obj, xmlnsEmpty);
            return sw.ToString();
        }

        public static T Deserialize(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringReader sr = new StringReader(xml);
            XmlReader reader = XmlReader.Create(sr, new XmlReaderSettings()
            {
                ConformanceLevel = ConformanceLevel.Fragment,
            }
            );
            return (T)serializer.Deserialize(reader);
        }
    }

    public class JsonTypedSerializer<T>
    {
        public static T Deserialize(string json)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(ms);
            }
        }

        public static string Serialize<U>(U obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }

    [XmlRoot("xml")]
    [DataContract]
    public class WechatBaseMessage /*: IXmlSerializable*/
    {
        #region Unix Time conversion
        public static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        public static long ToUnixTime(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }
        #endregion

        public WechatBaseMessage()
        {
            XmlDocument doc = new System.Xml.XmlDocument();
            MsgTypeCData = doc.CreateCDataSection("");
            ToUserNameCData = doc.CreateCDataSection("");
            FromUserNameCData = doc.CreateCDataSection("");
        }

        #region ToUserName
        [XmlElement("ToUserName")]
        [DataMember]
        public XmlCDataSection ToUserNameCData { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public string ToUserName
        {
            get
            {
                return ToUserNameCData.Value;
            }
            set
            {
                ToUserNameCData.Value = value;
            }
        }
        #endregion

        #region FromUserName
        [XmlElement("FromUserName")]
        [DataMember]
        public XmlCDataSection FromUserNameCData { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public string FromUserName
        {
            get
            {
                return FromUserNameCData.Value;
            }
            set
            {
                FromUserNameCData.Value = value;
            }
        }
        #endregion

        #region CreateTime
        [XmlElement("CreateTime")]
        [DataMember]
        public long CreateTimeData
        {
            get
            {
                return ToUnixTime(CreateTime);
            }
            set
            {
                CreateTime = FromUnixTime(value);
            }
        }

        [XmlIgnore]
        [IgnoreDataMember]
        public DateTime CreateTime { get; set; }
        #endregion

        #region MsgType
        [XmlElement("MsgType")]
        [DataMember]
        public XmlCDataSection MsgTypeCData { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public string MsgType
        {
            get
            {
                return MsgTypeCData.Value;
            }
            set
            {
                MsgTypeCData.Value = value;
            }
        }
        #endregion

        public virtual string ToXml()
        {
            return XmlTypedSerializer<WechatBaseMessage>.Serialize(this);
        }

        public static T FromXml<T>(string xml) where T : WechatBaseMessage
        {
            //Deserialize as base message type to determine the message type.
            T message = XmlTypedSerializer<T>.Deserialize(xml);
            return message;
        }
    }
}
