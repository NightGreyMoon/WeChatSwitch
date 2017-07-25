using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeChatSwitch.DAL;

namespace WeChatSwitch.BLL
{
    public class LogManager : BaseManager
    {
        public LogManager()
        {
            Context = new WebChatSwitchEntities();
        }

        public bool AddLog(SystemLog log)
        {
            Context.SystemLogs.Add(log);
            int result = Context.SaveChanges();
            return (result == 1);
        }
    }
}
