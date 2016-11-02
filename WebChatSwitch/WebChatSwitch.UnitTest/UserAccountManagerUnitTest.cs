using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebChatSwitch.BLL;

namespace WebChatSwitch.UnitTest
{
    [TestClass]
    public class UserAccountManagerUnitTest
    {
        [TestMethod]
        public void BasicDataAccessTest()
        {
            UserAccountManager manager = new UserAccountManager();
            var account = manager.GetUserAccountInfoByOpenId("haobohaobohaobohaobohaobo123");
            Assert.IsNotNull(account);
            Assert.AreNotEqual(0, account.Items.Count);
        }
    }
}
