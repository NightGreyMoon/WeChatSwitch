using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebChatSwitch.DAL;

namespace WebChatSwitch.BLL
{
    public class ItemManager : BaseManager
    {
        public ItemManager()
        {
            Context = new WebChatSwitchEntities();
        }

        public List<Item> GetAvailableItems(string searchKeyword)
        {
            var items = Context.Items.Include("ItemPictures").Include("UserAccount").Where(i =>
                (i.Title.Contains(searchKeyword) || i.Description.Contains(searchKeyword)) && i.Available).ToList();
            return items;
        }

        public List<Item> GetAllAvailableItems()
        {
            var items = Context.Items.Include("ItemPictures").Include("UserAccount").Where(i => i.Available).ToList();
            return items;
        }

        public bool SaveNewItem(Item item)
        {
            Context.Items.Add(item);
            UserAccount account = Context.UserAccounts.First(ua => ua.Id == item.OwnerId);
            account.Balance = (short)(account.Balance - 1);
            int result = Context.SaveChanges();
            return (result > 0);
        }
    }
}
