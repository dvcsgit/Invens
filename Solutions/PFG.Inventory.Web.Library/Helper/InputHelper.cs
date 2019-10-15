using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace PFG.Inventory.Web.Library.Helper
{
    public static class InputHelper
    {
        /// <summary>
        /// http://lastattacker.blogspot.tw/2011/11/mvc-2-hidden-helper-extension.html
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString HiddenEntirelyFor<TModel, TResult>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression)
        {
            ModelMetadata modelMetadata =
               ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            List<string> htmlEntries =
               modelMetadata.Properties
                  .Select(property => htmlHelper.Hidden(ExpressionHelper.GetExpressionText(expression) + "." + property.PropertyName, property.Model, null))
                  .Select(mvcHtmlString => mvcHtmlString.ToHtmlString())
                  .ToList();
            return MvcHtmlString.Create(String.Join(Environment.NewLine, htmlEntries.ToArray()));
        }
    }

}
