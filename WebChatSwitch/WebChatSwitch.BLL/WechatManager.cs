using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebChatSwitch.DAL;

namespace WebChatSwitch.BLL
{
    public class WechatManager : BaseManager
    {
        public WechatManager()
        {
            Context = new WebChatSwitchEntities();
        }

        public WechatCache GetWechatCache()
        {
            DateTime compare = DateTime.Now.AddMinutes(-1.5);
            WechatCache cache = Context.WechatCaches.FirstOrDefault(i => i.Timestamp > compare);
            return cache;
        }

        public void UpdateWechatCache(WechatCache cache)
        {
            Context.WechatCaches.RemoveRange(Context.WechatCaches.AsEnumerable());
            Context.WechatCaches.Add(cache);
            Context.SaveChanges();
        }
    }
}
