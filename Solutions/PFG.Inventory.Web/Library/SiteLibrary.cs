using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Mvc.Ajax;
using NLog;
using PagedList.Mvc;



namespace PFG.Inventory.Web.Library
{
    public class SiteLibrary
    {
        protected static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// mvcpager3 配置
        /// </summary>
        /// <returns></returns>
        //public static PagerOptions GetDefaultPagerOptions()
        //{
        //    var _strFormat = "<li>{0}</li>";
        //    return new PagerOptions()
        //    {
        //        AutoHide = true,
        //        PageIndexParameterName = "pageNo",
        //        FirstPageText = "首頁",
        //        PrevPageText = "上一頁",
        //        NextPageText = "下一頁",
        //        LastPageText = "尾頁",
        //        CssClass = "pagination pull-right",
        //        ContainerTagName = "ul",
        //        CurrentPagerItemWrapperFormatString = "<li class='active'><a href='#'>{0}</a></li>",
        //        NumericPagerItemWrapperFormatString = _strFormat,
        //        MorePagerItemWrapperFormatString = _strFormat,
        //        PagerItemWrapperFormatString = _strFormat,
        //        //SeparatorHtml = "",
        //        ShowMorePagerItems = false,
        //        NumericPagerItemCount = 5
        //    };
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static PagedListRenderOptions GetDefaultPagerOptions(bool isAjax)
        {

            PagedListRenderOptions result = null;
            var options = new PagedListRenderOptions() {
                DisplayLinkToFirstPage = PagedListDisplayMode.Always,
                DisplayLinkToLastPage = PagedListDisplayMode.Always,
				DisplayLinkToPreviousPage = PagedListDisplayMode.Always,
				DisplayLinkToNextPage = PagedListDisplayMode.Always,
                DisplayLinkToIndividualPages = true,
				ContainerDivClasses = null,
                //UlElementClasses = new[] {"pager"},
				ClassToApplyToFirstListItemInPager = null,
				ClassToApplyToLastListItemInPager = null,
                LinkToPreviousPageFormat = "上一頁",
                LinkToNextPageFormat = "下一頁",
                LinkToFirstPageFormat = "首頁",
                LinkToLastPageFormat = "尾頁"
            };
           
            if(isAjax)
            {
                result = PagedListRenderOptions.EnableUnobtrusiveAjaxReplacing(options, GetDefaultAjaxOptions());
            }
            else
            {
                result = options;
            }

            return options;
        }

        /// <summary>
        /// 預設的Ajax From Options
        /// </summary>
        /// <returns></returns>
        public static AjaxOptions GetDefaultAjaxOptions()
        {
            var updateTargetId = "divGridView";
            return new AjaxOptions
            {
                UpdateTargetId = updateTargetId,
                InsertionMode = InsertionMode.Replace,
                OnBegin = "$.ShowGridLoading('" + updateTargetId + "')",
                OnComplete = "$.HideGridLoading('" + updateTargetId + "')"
            };
        }

        /// <summary>
        /// 取得Web.config中的AppSetting參數
        /// Usage: SiteLibrary.AppSettings("key")
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String AppSettings(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static IEnumerable<Enum> GetFlags(Enum input)
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return value;
        }

        /// <summary>
        /// 取得指定附檔名的 MIME type string
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        public static string GetMIMETypeString(string fileExtension)
        {
            // 附檔名要是如 .bmp 格式
            fileExtension = fileExtension.Trim();
            if (fileExtension.StartsWith("."))
                fileExtension = "." + fileExtension;

            // 取得 MIME type string
            try
            {
                String mime = "application/octetstream";
                Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("." + fileExtension);
                if (rk != null && rk.GetValue("Content Type") != null)
                    mime = rk.GetValue("Content Type").ToString();
                return mime;
            }
            catch
            {
                return "application/octetstream";
            }
        }

        /// <summary>
        /// 產生Sorting Column 的class樣式
        /// </summary>
        /// <param name="field"></param>
        /// <param name="sortingField"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static string SortingClassColumn(Enum field, Enum sortingField, Enum direction)
        {
            var sortClass = "sorting";
            if (field.Equals(sortingField))
            {
                
                var directionStr = direction.ToString().ToUpper();
                
                if (directionStr == "ASC")
                {
                    sortClass = "sorting_asc";
                }
                else
                {
                    sortClass = "sorting_desc";
                }
                
            }

            return sortClass;
        }

        /// <summary>
        /// 取得設定檔 設定的PowerUser
        /// </summary>
        /// <returns></returns>
        public static List<string> GetPowerUserList()
        {
            var result = new List<string>();
            try
            {
                var tempPowerUser = SiteLibrary.AppSettings("PowerUser");

                result = tempPowerUser.Split(',').ToList();
            }
            catch (Exception ex)
            {

                _logger.Error("發生錯誤:{0}", ex.Message);
            }
            return result;
        } 

        /// <summary>
        /// 判斷目前的使用者 是不是 超級使用者
        /// </summary>
        /// <returns></returns>
        public static bool IsPowerUser()
        {
            bool result = false;
            var userData = UserUtils.GetUserInfo();

            if (userData != null)
            {
                var powerUserList = SiteLibrary.GetPowerUserList();
                result = powerUserList.Contains(userData.UserId);
            }

            return result;
        }

    }
}
