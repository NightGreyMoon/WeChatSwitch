using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeChatSwitch.DAL;

namespace WeChatSwitch.BLL
{
    public class WechatManager : BaseManager
    {
        public WechatManager()
        {
            Context = new WebChatSwitchEntities();
        }

        public WechatCache GetWechatCache()
        {
            DateTime compare = DateTime.UtcNow.AddMinutes(-100);
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
