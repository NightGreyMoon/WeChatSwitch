using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wechat.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string rawXml = "<xml><ToUserName><![CDATA[gh_b42ee465d7d6]]></ToUserName><FromUserName><![CDATA[oQQZHv12nxTUlP4ybQ4bagKHMj0Y]]></FromUserName><CreateTime>1461210306</CreateTime><MsgType><![CDATA[event]]></MsgType><Event><![CDATA[scancode_waitmsg]]></Event><EventKey><![CDATA[rselfmenu_0_0]]></EventKey><ScanCodeInfo><ScanType><![CDATA[barcode]]></ScanType><ScanResult><![CDATA[EAN_13,6911316223400]]></ScanResult></ScanCodeInfo></xml>";
            //string rawXml = "<xml><ToUserName><![CDATA[gh_b42ee465d7d6]]></ToUserName><FromUserName><![CDATA[oQQZHv12nxTUlP4ybQ4bagKHMj0Y]]></FromUserName><CreateTime>1461210306</CreateTime><MsgType><![CDATA[event]]></MsgType><Event><![CDATA[scancode_waitmsg]]></Event><EventKey><![CDATA[rselfmenu_0_0]]></EventKey></xml>";
            WechatScanMessage posMessage = WechatBaseMessage.FromXml<WechatScanMessage>(rawXml);
            Assert.AreNotEqual(null, posMessage);
        }
    }
}
