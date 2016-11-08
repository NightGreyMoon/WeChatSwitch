using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WechatPublicAccount
{
    public delegate int RefreshAccessToken(WeiXinAccount currentAccount);
    public class WeiXinAccount : IDisposable
    {
        public event RefreshAccessToken OnAccessTokenExpired;
        public string AccessToken { get; set; }
        public string AccountID { get; set; }
        public string Secret { get; set; }

        public int Duration = 1;

        public int Counter = 0;

        public bool IsAutoMoniter = true;

        private Thread MoniterThread = null;

        public bool IsAvaiable { get; set; }

        public WeiXinAccount(bool autoMonitor)
        {
            IsAutoMoniter = autoMonitor;
        }

        protected void StartMornitor()
        {
            if (IsAutoMoniter)
            {
                MoniterThread = new Thread(new ThreadStart(this.TimerMonitor_Elapsed));
                MoniterThread.Start();
            }
        }

        public void TimerMonitor_Elapsed()
        {
            Thread.Sleep(1000 * 10);
            while (true)
            {
                if (this.Counter >= this.Duration)
                {
                    if (OnAccessTokenExpired != null)
                    {
                        this.Duration = OnAccessTokenExpired(this);
                    }
                    this.Counter = 0;
                }

                Counter = Counter + 1;
                Thread.Sleep(1000);
            }
        }

        public void Dispose()
        {
            if (MoniterThread != null)
            {
                MoniterThread.Abort();
            }
        }
    }
}
