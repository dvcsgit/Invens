using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace PFG.Inventory.Web.Library.Extensions
{
    public static class DataTableExtensions
    {
        //ref:http://codereview.stackexchange.com/questions/30714/faster-way-to-convert-datatable-to-list-of-class
		/// <summary>
		/// 將DataTable 轉成List物件
        /// useage:
        /// DataTable dtTable = GetEmployeeDataTable();
        /// List<Employee> employeeList = dtTable.DataTableToList<Employee>();
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="table"></param>
		/// <returns></returns>
        public static List<T> DataTableToList<T>(this DataTable table) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();
                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();
                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }
    }
}
