using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using PFG.Inventory.Web.Library.Enums;

namespace PFG.Inventory.Web.Library.Models
{
    /// <summary>
    /// cookie存放物件
    /// </summary>
    public class UserInfo
    {
        public string DisplayName { get; set; }
        public string UserIdentifier { get; set; }
        public string UserId { get; set; }
        public string RoleName { get; set; }
        public List<string> Roles { get; set; }


        /// <summary>
        /// 序列化成JSON
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// 還原成物件
        /// </summary>
        /// <param name="userContextData"></param>
        /// <returns></returns>
        public static UserInfo FromString(string userContextData)
        {
            return JsonConvert.DeserializeObject<UserInfo>(userContextData);
        }

        public UserInfo()
        {
            this.Roles = new List<string>();
        }
    }
}
