using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wechat
{
    public class JsInitResponse
    {
        public string appId { get; set; }
        public string timestamp { get; set; }
        public string nonceStr { get; set; }
        public string signature { get; set; }
    }
}
