using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using PFG.Inventory.Domain.Resources;

namespace PFG.Inventory.Web.Library
{
    public static class SQLiteExtensions
    {
        /// <summary>
        /// 將List資料產生Insert Scripts
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="tableName">Table名稱</param>
        /// <returns></returns>
        public static List<string> ToInsertScript<T>(this IEnumerable<T> source, string tableName)
        {
            var result = new List<string>();
            var datatype = typeof(T);
            var propertyInfos = datatype.GetProperties();
            List<string> columnName = propertyInfos.Select(x => x.Name).ToList();
            var tempColumnName = columnName.Select(x => "[" + x + "]").ToList();//避免保留字問題
            string strColumn = string.Join(",", tempColumnName);
            foreach (var item in source)
            {
                List<string> columnValue = new List<string>();
                var sourceData = item.GetType();
                foreach (var subItem in columnName)
                {
                    var propertyValue = sourceData.GetProperty(subItem).GetValue(item, null);
                    if (propertyValue != null)
                    {
                        columnValue.Add("'" + propertyValue.ToString().Replace("'", "’").Replace(@"\", "＼") + "'");
                    }
                    else
                    {
                        columnValue.Add("''");
                    }
                }
                var strColumnValue = string.Join(",", columnValue);
                var strSQL = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", tableName, strColumn, strColumnValue);

                result.Add(strSQL);
            }
            return result;
        }

        /// <summary>
        /// For MS SQL 暫時使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static List<string> ToInsertScriptForMS<T>(this IEnumerable<T> source, string tableName)
        {
            var result = new List<string>();
            var datatype = typeof(T);
            var propertyInfos = datatype.GetProperties();
            List<string> columnName = propertyInfos.Select(x => x.Name).ToList();
            var tempColumnName = columnName.Select(x => "[" + x + "]").ToList();//避免保留字問題
            string strColumn = string.Join(",", tempColumnName);
            foreach (var item in source)
            {
                List<string> columnValue = new List<string>();
                var sourceData = item.GetType();

                foreach (var subItem in columnName)
                {
                    var propertyValue = sourceData.GetProperty(subItem).GetValue(item, null);
                    var proType = sourceData.GetProperty(subItem).PropertyType;
                    var nullableFlag = false;
                    if (proType.Name == "Nullable`1")
                    {
                        nullableFlag = true;
                    }

                    //判斷時間
                    if (proType.FullName.Contains("System.DateTime"))
                    {
                        if(nullableFlag)
                        {
                            var tempDate = (DateTime?)sourceData.GetProperty(subItem).GetValue(item, null);
                            if(tempDate.HasValue)
                            {
                                propertyValue = tempDate.Value.ToString(Resources.StrDateTimeFormat);
                            }
                        }
                        else
                        {
                            var tempDate = (DateTime)sourceData.GetProperty(subItem).GetValue(item, null);
                            propertyValue = tempDate.ToString(Resources.StrDateTimeFormat);
                        }
                    }

                    if (propertyValue != null)
                    {
                        columnValue.Add("'" + propertyValue.ToString().Replace("'", "’").Replace(@"\", "＼") + "'");
                    }
                    else
                    {

                        if (nullableFlag)
                        {
                            columnValue.Add("NULL");
                        }
                        else
                        {
                            columnValue.Add("''");
                        }
                    }
                }
                var strColumnValue = string.Join(",", columnValue);
                var strSQL = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", tableName, strColumn, strColumnValue);

                result.Add(strSQL);
            }
            return result;
        }

        /// <summary>
        /// 產生CreateTable語法 (預設會將原TABLE給DROP)
        /// 會處理以下屬性
        /// 必填 [RequiredAttribute]
        /// 長度 [MaxLength(30)]
        /// 欄位型態 [Column(TypeName = "VARCHAR")]
        /// TODO精度未實作完善
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToCreateTableScript<T>(this IEnumerable<T> source, string tableName)
        {
            var result = "";
            var template = "DROP TABLE IF EXISTS {0};CREATE TABLE IF NOT EXISTS [{0}] ({1});";
            var columnList = new List<string>();
            var datatype = typeof(T);
            var propertyInfos = datatype.GetProperties();
            foreach (var item in propertyInfos)
            {
                
                var tempColumn = "";
                //欄位名稱
                tempColumn += "[" + item.Name + "]";


                var cusAttrs = item.GetCustomAttributes(false);//取得所有自訂屬性
                var proType = item.PropertyType;
                //型態(一定會有形態)
                var tempColumnType = "";
                var columnAttr = cusAttrs.Where(x => x.GetType() == typeof(ColumnAttribute)).FirstOrDefault() as ColumnAttribute;
                if(columnAttr!=null)
                {
                    tempColumnType = columnAttr.TypeName;
                }
                else
                {
                    //沒有強制指定Column Type則使用預設所偵測到的反射型態
                    switch (proType.Name)
                    {
                        case "String":
                            tempColumnType = "NVARCHAR";
                            break;
                        case "Int32":
                            tempColumnType = "INT";
                            break;
                        case "Float":
                        case "Decimal":
                            tempColumnType = "NUMERIC";
                            break;
                        case "DateTime":
                            tempColumnType = "DATETIME";
                            break;
                        case "Nullable`1":
                            if(proType.FullName.Contains("Decimal"))
                            {
                                tempColumnType = "NUMERIC";
                            }
                            break;
                        default:
                            tempColumnType = "TEXT";
                            break;
                    }
                }

                tempColumn += " " + tempColumnType;

                //長度
                var maxLengthAttr = cusAttrs.Where(x => x.GetType() == typeof(MaxLengthAttribute)).FirstOrDefault() as MaxLengthAttribute;
                if (maxLengthAttr != null)
                {
                    var length = maxLengthAttr.Length;
                    tempColumn += " (" + length + ")";
                }

                //精度
                var precisionAttr = cusAttrs.Where(x => x.GetType() == typeof(PrecisionAttribute)).FirstOrDefault() as PrecisionAttribute;
                if (precisionAttr != null)
                {
                    tempColumn += " (" + precisionAttr.Precision + "," + precisionAttr.Scale + ")";
                }

                //必填與否
                var requiredAttr = cusAttrs.Where(x => x.GetType() == typeof(RequiredAttribute)).FirstOrDefault() as RequiredAttribute;
                if (requiredAttr != null)
                {
                    tempColumn += " NOT NULL";
                }

                columnList.Add(tempColumn);
            }
 
            result = string.Format(template, tableName, string.Join(",", columnList));
            return result;
        }
    }
}