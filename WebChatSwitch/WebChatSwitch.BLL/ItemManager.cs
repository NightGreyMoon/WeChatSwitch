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
            var items = Context.Items.Include("ItemPictures").Where(i =>
                (i.Title.Contains(searchKeyword) || i.Description.Contains(searchKeyword)) && i.Available).ToList();
            return items;
        }
    }
}
