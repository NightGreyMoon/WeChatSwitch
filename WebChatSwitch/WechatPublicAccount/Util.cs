﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WechatPublicAccount
{
    public class Util
    {
        public static long CreateTimestamp()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }

        public static DateTime UtcToChinaTime(DateTime utcTime)
        {
            TimeZoneInfo chinaTimeZone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(p => p.Id == "China Standard Time");
            if (chinaTimeZone != null)
            {
                return TimeZoneInfo.ConvertTimeFromUtc(utcTime, chinaTimeZone);
            }
            else
            {
                return utcTime;
            }
        }

        public static string UtcToChinaTime(object utcTimeObject, string format = "yyyy/MM/dd HH:mm:ss")
        {
            if (utcTimeObject == null) return null;
            DateTime utcTime;
            if (DateTime.TryParse(utcTimeObject.ToString(), out utcTime))
            {
                return UtcToChinaTime(utcTime).ToString(format);
            }

            return null;
        }

        public static string UtcToClientTime(object utcTimeObject, int utcOffset, string format = "yyyy/MM/dd HH:mm:ss")
        {
            if (utcTimeObject == null) return "";
            DateTime utcTime;
            if (DateTime.TryParse(utcTimeObject.ToString(), out utcTime))
            {
                return utcTime.AddHours(utcOffset).ToString(format);
            }
            return "";
        }
    }
}
