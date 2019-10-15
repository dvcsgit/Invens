using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NLog;

namespace PFG.Inventory.Web.Library.Extensions
{
    public static class StringExtensions
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 把過長的文字切斷
        /// </summary>
        /// <param name="source"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string TextEllipsis(this string source, int length)
        {
            string strResult = source.ToString();
            if (strResult.Length > length)
                return strResult.Substring(0, length) + "...";
            else
                return strResult;
        }

        /// <summary>
        /// 把時間字串切成看的懂的
        /// 1碼 => 0 => 00:00
        /// 2碼 => 15 => 00:18
        /// 3碼 => 300 => 03:00
        /// 4碼 => 1800 => 18:00
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ConvertTimeCanRead(this short source)
        {
            string result = source.ToString("D4");
            try
            {
                var min = result.Substring(0, 2);
                var sec = result.Substring(2, 2);
                result = string.Format("{0}:{1}", min, sec);
            }
            catch (Exception ex)
            {
                _logger.Error("發生錯誤:{0}", ex);
            }
            return result;
        }

        /// <summary>
        /// 字串轉成時間格式
        /// </summary>
        /// <returns></returns>
        public static DateTime StrToDateTime(this string source, string format)
        {
            DateTime result;
            CultureInfo provider = CultureInfo.InvariantCulture;
            result = DateTime.ParseExact(source, format, provider);

            return result;
        }

        /// <summary>
        /// 將字串左邊補0
        /// 當來源是 空值 或 "" 則不會處理
        /// </summary>
        /// <param name="source"></param>
        /// <param name="totalWidth">總長度</param>
        /// <returns></returns>
        public static string PadLeftWithZero(this string source, int totalWidth)
        {
            string result = source;
            if (!string.IsNullOrEmpty(result))
            {
                result = source.PadLeft(totalWidth, '0');
            }
            return result;
        }

        /// <summary>
        /// 將字串中從右邊取5碼
        /// 少於5碼則忽略
        /// 00005990
        /// </summary>
        /// <param name="source"></param>
        /// <param name="totalWidth"></param>
        /// <returns></returns>
        public static string Right(this string source, int length)
        {

            string result = source;
            if (!string.IsNullOrEmpty(result))
            {
                if (result.Length > length)
                    result = source.Substring(result.Length - length, length);
            }
            return result;

        }

    }
}
