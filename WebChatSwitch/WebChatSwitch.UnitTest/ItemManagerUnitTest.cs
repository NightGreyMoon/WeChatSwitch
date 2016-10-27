using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebChatSwitch.BLL;
using System.Linq;

namespace WebChatSwitch.UnitTest
{
    [TestClass]
    public class ItemManagerUnitTest
    {
        [TestMethod]
        public void GetAvailableItemsTest()
        {
            ItemManager manager = new ItemManager();
            var items = manager.GetAvailableItems("4");
            Assert.AreNotEqual(0, items.Count);
            Assert.IsNotNull(items.FirstOrDefault());
            Assert.AreNotEqual(0, items.FirstOrDefault().ItemPictures.Count);
            Assert.IsNotNull(items.FirstOrDefault().ItemPictures.FirstOrDefault());
        }
    }
}
