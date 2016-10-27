using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebChatSwitch.DAL;

namespace WebChatSwitch.BLL
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
