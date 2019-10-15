using System;
using System.Collections.Generic;

using System.Linq;
using System.Web;

namespace PFG.Inventory.Web.Library
{
    /// <summary>
    /// REF:http://aspdotnetdevelopment.wordpress.com/2013/12/09/entity-framework-code-first-decimal-precision-dataannotation-attribute/
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PrecisionAttribute : Attribute
    {
        public int Precision { get; set; }
        public int Scale { get; set; }

        public PrecisionAttribute(int precision, int scale)
        {
            this.Precision = precision;
            this.Scale = scale;
        }

        //public static void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Properties().Where(x => x.GetCustomAttributes(false).OfType(Precision).Any()).Configure(c => c.HasPrecision(c.ClrPropertyInfo.GetCustomAttributes(false).OfType(Precision).First().precision, c.ClrPropertyInfo.GetCustomAttributes(false).OfType(Precision).First().scale));
        //}
    }
}