using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace PFG.Inventory.Web.Library
{
     public static class DateTimeExtensions
    {
        public static string ToChineseDate(this DateTime source)
        {
            //設定台灣日曆 
            TaiwanCalendar tc = new TaiwanCalendar();
            return string.Format("{0}{1:MMdd}", tc.GetYear(source), source);
        }
    }
}