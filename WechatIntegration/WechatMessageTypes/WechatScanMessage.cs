using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Wechat
{
    [XmlRoot("xml")]
    [DataContract]
    public class WechatScanMessage : WechatBaseMessage
    {
        #region Event
        [XmlElement("Event")]
        [DataMember]
        public XmlCDataSection EventCData { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public string Event
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
        #endregion

        [XmlElement("ScanCodeInfo")]

        public ScanCodeInfo ScanCodeInfo { get; set; }
    }


    public class ScanCodeInfo
    {
        #region ScanType
        [XmlElement("ScanType")]
        [DataMember]
        public XmlCDataSection ScanTypeCData { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public string ScanType
        {
            get
            {
                return ScanTypeCData.Value;
            }
            set
            {
                ScanTypeCData.Value = value;
            }
        }
        #endregion

        #region ScanResult
        [XmlElement("ScanResult")]
        [DataMember]
        public XmlCDataSection ScanResultCData { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public string ScanResult
        {
            get
            {
                return ScanResultCData.Value;
            }
            set
            {
                ScanResultCData.Value = value;
            }
        }
        #endregion
    }
}
