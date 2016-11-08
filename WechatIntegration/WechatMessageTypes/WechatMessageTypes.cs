using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Wechat
{
    [XmlRoot("xml")]
    [DataContract]
    public class WechatTextMessage : WechatBaseMessage
    {
        [XmlElement("Content")]
        [DataMember]
        public XmlCDataSection ContentCData { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public string Content
        {
            get
            {
                return ContentCData.Value;
            }
            set
            {
                ContentCData.Value = value;
            }
        }

        public WechatTextMessage()
        {
            MsgType = "text";
            XmlDocument doc = new System.Xml.XmlDocument();
            ContentCData = doc.CreateCDataSection("");
        }
    }

    [XmlRoot("xml")]
    [DataContract]
    public class WechatEventMessage : WechatBaseMessage
    {
        [XmlElement("Event")]
        [DataMember]
        public XmlCDataSection EventCData { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public string EventType
        {
            get
            {
                return EventCData.Value;
            }
            set
            {
                EventCData.Value = value;
            }
        }

        [XmlElement("EventKey")]
        [DataMember]
        public XmlCDataSection EventKeyCData { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public string EventKey
        {
            get
            {
                return EventKeyCData.Value;
            }
            set
            {
                EventKeyCData.Value = value;
            }
        }

        public WechatEventMessage()
        {
            MsgType = "event";
            XmlDocument doc = new System.Xml.XmlDocument();
            EventCData = doc.CreateCDataSection("");
            EventKeyCData = doc.CreateCDataSection("");
        }
    }

    [XmlRoot("xml")]
    [DataContract]
    public class WechatTemplateMsgEventMessage : WechatBaseMessage
    {
        [XmlElement("Event")]
        [DataMember]
        public XmlCDataSection EventCData { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public string EventType
        {
            get
            {
                return EventCData.Value;
            }
            set
            {
                EventCData.Value = value;
            }
        }

        [XmlElement("MsgID")]
        [DataMember]
        public string MsgId { get; set; }


        [XmlElement("Status")]
        [DataMember]
        public XmlCDataSection StatusCData { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public string Status
        {
            get
            {
                return StatusCData.Value;
            }
            set
            {
                StatusCData.Value = value;
            }
        }

        public WechatTemplateMsgEventMessage()
        {
            MsgType = "event";
            XmlDocument doc = new System.Xml.XmlDocument();
            EventCData = doc.CreateCDataSection("");
            StatusCData = doc.CreateCDataSection("");
        }
    }


    [XmlRoot("xml")]
    [DataContract]
    public class WechatImageMessage : WechatBaseMessage
    {
        [XmlElement("MediaId")]
        [DataMember]
        public XmlCDataSection MediaIdCData { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public string MediaId
        {
            get
            {
                return MediaIdCData.Value;
            }
            set
            {
                MediaIdCData.Value = value;
            }
        }

        [XmlElement("PicUrl")]
        [DataMember]
        public XmlCDataSection PicUrlCData { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public string PicUr
        {
            get
            {
                return PicUrlCData.Value;
            }
            set
            {
                PicUrlCData.Value = value;
            }
        }

        public WechatImageMessage()
        {
            MsgType = "image";
        }
    }

    [XmlRoot("xml")]
    [DataContract]
    public class WechatVoiceMessage : WechatBaseMessage
    {
        public WechatVoiceMessage()
        {
            MsgType = "voice";
            throw new NotImplementedException();
        }
    }
}
