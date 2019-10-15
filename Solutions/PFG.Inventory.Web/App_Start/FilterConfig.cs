﻿using System.Web;
using System.Web.Mvc;
using PFG.Inventory.Web.Filters;

namespace PFG.Inventory.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new UserPermissionAttribute());
        }
    }
}
