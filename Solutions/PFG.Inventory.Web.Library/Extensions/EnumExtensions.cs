using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PFG.Inventory.Web.Library.Extensions
{
    public static class EnumExtensions<T>
    {
        /// <summary>
        /// Parses the specified value.
        /// 參考文章︰http://geekswithblogs.net/sdorman/archive/2007/09/25/Generic-Enum-Parsing-with-Extension-Methods.aspx
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        /// <summary>
        /// Gets the values.
        /// 參考文章︰http://geekswithblogs.net/sdorman/archive/2007/09/25/Generic-Enum-Parsing-with-Extension-Methods.aspx
        /// </summary>
        /// <returns></returns>
        public static IList<T> GetValues()
        {
            IList<T> list = new List<T>();
            foreach (object value in Enum.GetValues(typeof(T)))
            {
                list.Add((T)value);
            }
            return list;
        }

        /// <summary>
        /// 驗證列舉是否再合理範圍內
        /// </summary>
        public static bool IsValidRange<T2>(T2 enumValue)
        {
            int maxEnumValue = 0;
            foreach (T aEnumValue in EnumExtensions<T>.GetValues())
                maxEnumValue += Convert.ToInt32(Enum.Format(typeof(T), aEnumValue, "d"));

            if (Convert.ToInt32((T2)enumValue) > maxEnumValue)
                return false;

            return true;
        }

        /// <summary>
        /// 舉出 flag 列舉的所有值
        /// </summary>
        /// <returns></returns>
        public static T GetAllValues()
        {
            int allValue = 0;
            foreach (T aEnumValue in EnumExtensions<T>.GetValues())
                allValue += Convert.ToInt32(Enum.Format(typeof(T), aEnumValue, "d"));

            return EnumExtensions<T>.Parse(allValue.ToString());
        }


        /// <summary>
        /// 取得 enum 的描述 (descritpion)
        /// 
        /// 範例︰        
        /// enum Foo
        /// {
        ///     [Description("Bright Pink")]
        ///     BrightPink = 2
        /// }
        /// 
        /// 當呼叫 GetDescription(Foo.BrightPink)，本方法會回傳 "Bright Pink" 描述        
        /// 
        /// 參考自︰http://weblogs.asp.net/grantbarrington/archive/2009/01/19/enumhelper-getting-a-friendly-description-from-an-enum.aspx
        /// </summary>
        /// <param name="en">The Enumeration</param>
        /// <returns>A string representing the friendly name</returns>
        public static string GetDescription(T en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return en.ToString();
        }


        /// <summary>
        /// 將描述轉回 Enum 型態
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        public static T DescriptionToValue(string description)
        {
            foreach (var aEnum in EnumExtensions<T>.GetValues())
            {
                if (EnumExtensions<T>.GetDescription(aEnum).Equals(description, StringComparison.CurrentCultureIgnoreCase))
                {
                    return aEnum;
                }
            }

            throw new Exception("無此描述值！");
        }

        /// <summary>
        /// 取得Enum 字串值
        /// 據說效能比 enum.ToString()好
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string GetEnumString(T en)
        {
            var result = "";
            try
            {
                result = Enum.GetName(typeof(T), en);
            }
            catch (Exception ex)
            {

            }
            return result;
        }

    }
}
