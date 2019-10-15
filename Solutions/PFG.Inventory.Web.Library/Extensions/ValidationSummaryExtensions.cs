using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace PFG.Inventory.Web.Library.Extensions
{
    public static class ValidationSummaryExtension
    {
        public static string ValidationSummaryWithContainer(this HtmlHelper ext, string message)
        {
            return ValidationSummaryWithContainer(ext, message, "");
        }

        /// <summary>
        /// Creates a validation summary with a container element 
        /// surrounding the summary and error messages.
        /// </summary>        
        public static string ValidationSummaryWithContainer(this HtmlHelper ext, string message, string subMessage)
        {
            //<button class="close" data-dismiss="alert">×</button>
            //<h4 class="alert-heading">請修正以下的錯誤後，再試一次.</h4>
            var summaryoutput = ext.ValidationSummary("");
            if (!string.IsNullOrEmpty(summaryoutput.ToHtmlString()))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<div class='alert alert-block alert-error fade in validation-summary-valid'>");
                sb.Append("<button class='close' data-dismiss='alert'>×</button>");
                sb.Append(string.Format("<h4 class='alert-heading'>{0}</h4>", message));
                sb.Append(summaryoutput);
                if (!string.IsNullOrEmpty(subMessage))
                    sb.Append(subMessage);
                sb.Append("</div>");
                return sb.ToString();
            }
            else
                return string.Empty;
        }


    }
}
