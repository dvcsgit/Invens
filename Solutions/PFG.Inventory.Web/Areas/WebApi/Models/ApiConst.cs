using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PFG.Inventory.Web.Areas.WebApi.Models
{
    /// <summary>
    /// 紀錄log層級
    /// </summary>
    public class ApiLogTypeConst
    {
        public static readonly string Debug = "Debug";
        public static readonly string Warn = "Warn";
        public static readonly string Info = "Info";
        public static readonly string Error = "Error";

    }
}