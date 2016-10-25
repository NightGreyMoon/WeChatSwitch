using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Wechat
{
    public partial class WechatLib
    {
        [DataContract]
        public class Error
        {
            [DataMember(Name = "errcode", EmitDefaultValue = true)]
            public int ErrorCode;

            [DataMember(Name = "errmsg", EmitDefaultValue = true)]
            public string ErrorMessage;
        }
    }
}
