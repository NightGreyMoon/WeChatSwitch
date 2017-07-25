using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeChatSwitch.DAL;

namespace WeChatSwitch.BLL
{
    public class BaseManager
    {
        private WebChatSwitchEntities context;

        public WebChatSwitchEntities Context
        {
            get
            {
                return context;
            }

            set
            {
                context = value;
            }
        }

        public BaseManager()
        {
            Context = new WebChatSwitchEntities();
        }
    }
}
