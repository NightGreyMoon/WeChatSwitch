using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
using System.Text;

namespace WebChatSwitch.UnitTest
{
    [TestClass]
    public class WeChatSignatureUnitTest
    {
        [TestMethod]
        public void TestGenerateSignature()
        {
            string jsToken = "kgt8ON7yVITDhtdwci0qeTQIB0ds82IxTu3pDvW_gUq36d7r-Ql3r218Wcde01rADcAmMSluhsHAi-RqAzluCw";
            string nonceStr = "Wm3WZYTPz0wzccnW";
            string timestamp = "1478715577";
            string currentURL = "http://scrumoffice.azurewebsites.net/Item/Create";
            string source = string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", jsToken, nonceStr, timestamp, currentURL);
            //string signature = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(source, "SHA1");            //string source = "jsapi_ticket=kgt8ON7yVITDhtdwci0qeTQIB0ds82IxTu3pDvW_gUqD60Q9zCrv1YkvovovbWdlJzFtIPryUYqDTrPQ1Sk6ww&noncestr=Wm3WZYTPz0wzccnW&timestamp=1414587457&url=http://webchatswitch.azurewebsites.net/Item/Create";

            //建立SHA1对象
            SHA1 sha = new SHA1CryptoServiceProvider();
            //将mystr转换成byte[] 
            ASCIIEncoding enc = new ASCIIEncoding();            byte[] dataToHash = enc.GetBytes(source);
            //Hash运算
            byte[] dataHashed = sha.ComputeHash(dataToHash);
            //将运算结果转换成string
            string signature = BitConverter.ToString(dataHashed).Replace("-", "").ToLower();
            Assert.IsNotNull(signature);
        }
    }
}
