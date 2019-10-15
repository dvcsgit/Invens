using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PFG.Inventory.Web.Library.Common
{
    public class CustomException : Exception, ISerializable
    {
        public CustomException()
            : base("發生錯誤") { }
        public CustomException(string message)
            : base(message) { }
        public CustomException(string message, Exception inner)
            : base(message, inner) { }
        protected CustomException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
