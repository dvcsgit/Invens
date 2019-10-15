using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPOI;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using NPOI.HSSF.Util;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;

namespace PFG.Inventory.Web.Library
{
    public static class ExcelExtensions
    {
        /// <summary>
        /// 數據匯出
        /// </summary>
        /// <typeparam name="T">數據源實體</typeparam>
        /// <param name="source">數據源</param>
        /// <param name="fileName">匯出文件名</param>
        /// <param name="titleName">標題名</param>
        /// <param name="sheetName">Sheet名</param>
        /// <param name="titleList">欄位名</param>
        /// <param name="outColumn">設置要剔除的欄位索引集合</param>
        /// <returns>存儲位置</returns>
        public static string ExportExcel<T>(this IEnumerable<T> source, string fileName, string titleName, string sheetName, List<string> titleList,List<int> outColumn)
        {
            var dataType = typeof(T);
            var sourceList = source.ToList();
            var workBook = new HSSFWorkbook();
            var workSheet = workBook.CreateSheet(sheetName);
            workSheet.DisplayGridlines = false;
            //標題設置
            ICell titleCell = workSheet.CreateRow(0).CreateCell(0);
            var columnList = dataType.GetPropertyDisplayNames();
            var cout = columnList.Count();
            if(outColumn!=null)
            {
                cout = cout - outColumn.Count();
            }
            workSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, cout - 1));
            titleCell.SetCellValue(titleName);
            ICellStyle titleStyle = workBook.CreateCellStyle();
            titleStyle.Alignment = HorizontalAlignment.CENTER;
            titleCell.CellStyle = titleStyle;
            //欄位顯示設置
            IRow colTitleRow = workSheet.CreateRow(1);
            //自定義欄位
            if (titleList != null)
            {
                for (int i = 0; i < columnList.Count; i++)
                {
                    ICell cellTitleCell = colTitleRow.CreateCell(i);
                    cellTitleCell.SetCellValue(titleList.ElementAt(i));
                }
            }
            //未自定義欄位設置
            else
            {
                if (outColumn == null)
                {
                    for (int i = 0; i < columnList.Count; i++)
                    {
                        ICell cellTitleCell = colTitleRow.CreateCell(i);
                        cellTitleCell.SetCellValue(columnList.ElementAt(i));
                    }
                }else
                {
                    var k = 0;
                    for (int i = 0; i < columnList.Count; i++)
                    {
                        if (i == outColumn.ElementAt(k))
                        {
                            k += 1;
                            continue;
                        }
                        ICell cellTitleCell = colTitleRow.CreateCell(i);
                        cellTitleCell.SetCellValue(columnList.ElementAt(i));
                        
                    }
                }
            }
            if (outColumn == null)
            {
                //數據顯示設置
                for (int i = 0; i < source.Count(); i++)
                {
                    IRow contentRow = workSheet.CreateRow(i + 2);

                    for (int j = 0; j < columnList.Count; j++)
                    {

                        ICell contentCell = contentRow.CreateCell(j);
                        var values = sourceList.ElementAt(i).GetPropertyValues();
                        contentCell.SetCellValue(values.ElementAt(j));
                    }
                }
            }else
            {
                //數據顯示設置
                for (int i = 0; i < source.Count(); i++)
                {
                    IRow contentRow = workSheet.CreateRow(i + 2);
                    var k = 0;
                    for (int j = 0; j < columnList.Count; j++)
                    {
                        if(j==outColumn.ElementAt(k))
                        {
                            k += 1;
                            continue;
                        }
                        ICell contentCell = contentRow.CreateCell(j);
                        var values = sourceList.ElementAt(i).GetPropertyValues();
                        contentCell.SetCellValue(values.ElementAt(j));
                    }
                }

            }
            if (outColumn == null)
            {
                SetBorderStyle(1, source.Count() + 1, 0, columnList.Count() - 1, workBook, workSheet);

            }
            else
            {
                SetBorderStyle(1,source.Count()+1,0,columnList.Count-1-outColumn.Count(),workBook,workSheet);
            }
            //下載保存
            var savePath = Path.Combine(Path.GetTempPath(), fileName);
            FileStream fs = new FileStream(savePath, FileMode.Create);
            workBook.Write(fs);
            fs.Close();
            return savePath;
        }

        /// <summary>
        /// 獲得DisplayName所設定的名稱
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns>名稱值</returns>
        private static string GetDisplayName(this MemberInfo memberInfo)
        {
            var titleName = string.Empty;
            var attribute = memberInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false).FirstOrDefault();
            if (attribute != null)
            {
                titleName = (attribute as DisplayNameAttribute).DisplayName;
            }
            else
            {
                titleName = memberInfo.Name;
            }
            return titleName;
        }
        /// <summary>
        /// 獲得屬性名displayName的特性名
        /// </summary>
        /// <param name="type">類型</param>
        /// <returns>特性名的值</returns>
        private static List<string> GetPropertyDisplayNames(this Type type)
        {
            var titleList = new List<string>();
            var propertyInfos = type.GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                var titleName = propertyInfo.GetDisplayName();
                titleList.Add(titleName);
            }
            return titleList;
        }

        private static void SetBorderStyle(int startRow, int endRow, int startCol, int endCol, IWorkbook workBook, ISheet workSheet)
        {
            //for (int r = startRow; r <= endRow; r++)
            //{
            //    IRow row = workSheet.GetRow(r);
            //    for (int c = startCol; c <= endCol; c++)
            //    {
            //        ICellStyle style = workBook.CreateCellStyle();
            //        style.BorderBottom = BorderStyle.THIN;
            //        style.BorderLeft = BorderStyle.THIN;
            //        style.BorderRight = BorderStyle.THIN;
            //        style.BorderTop = BorderStyle.THIN;
            //        style.Alignment = HorizontalAlignment.CENTER;
            //        ICell cell = row.GetCell(c);
            //        cell.CellStyle = style;
            //        workSheet.AutoSizeColumn(c);
            //    }
            //}
            ICellStyle style = workBook.CreateCellStyle();
            style.BorderBottom = BorderStyle.THIN;
            style.BorderLeft = BorderStyle.THIN;
            style.BorderRight = BorderStyle.THIN;
            style.BorderTop = BorderStyle.THIN;
            for (int r = startRow; r <= endRow; r++)
            {
                IRow row = workSheet.GetRow(r);

                for (int c = startCol; c <= endCol; c++)
                {




                    if (r == 1)
                    {
                        style.Alignment = HorizontalAlignment.CENTER;
                        //var color = Color.FromArgb(80, 124, 209);
                        style.FillPattern = FillPatternType.SOLID_FOREGROUND;
                        style.FillForegroundColor = HSSFColor.ROYAL_BLUE.index;



                        IFont font = workBook.CreateFont();
                        font.Boldweight = (short)400;
                        font.Color = HSSFColor.WHITE.index;
                        font.FontHeightInPoints = (short)12;
                        style.SetFont(font);

                    }
                    if (r > 1)
                    {
                        //var color = Color.FromArgb(239, 243, 251);
                        style.WrapText = true;
                        style.VerticalAlignment = VerticalAlignment.CENTER;
                        if (r % 2 == 0)
                        {
                            style.FillPattern = FillPatternType.SOLID_FOREGROUND;
                            style.FillForegroundColor = HSSFColor.LIGHT_CORNFLOWER_BLUE.index;
                        }
                    }

                    ICell cell = row.GetCell(c);
                    cell.CellStyle = style;
                    workSheet.AutoSizeColumn(c);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataList"></param>
        /// <param name="fileName">檔名: myExcel.xls</param>
        /// <returns>filePath</returns>
        public static string ExportExcel<T>(this IEnumerable<T> dataList, string fileName, string sheetName)
        {
            //Create workbook
            var datatype = typeof(T);
            var workbook = new HSSFWorkbook();
            var worksheet = workbook.CreateSheet(string.Format("{0}", sheetName));
            //Insert titles
            var row = worksheet.CreateRow(0);
            var titleListMap = datatype.GetPropertyDisplayNamesMap();
            var wrapRowCount = 0;

            var cellIdx = 0;
            foreach (var item in titleListMap)
            {
                var cell = row.CreateCell(cellIdx);


                var title = item.Value;
                //cell.CellStyle = fontStyle;

                cell.SetCellValue(title.Name);

                cellIdx++;
            }

            //Insert data values
            InsertDataValues(dataList, workbook, worksheet, titleListMap);


            //自動篩選
           // var endRange = IntToAlphabet.IndexToColumn(titleListMap.Count) + "1";
            //var headerRange = CellRangeAddress.ValueOf("A1:" + endRange);
            //worksheet.SetAutoFilter(headerRange);

            //自動設寬
            for (int i = 1; i < titleListMap.Count + 1; i++)
            {
                worksheet.AutoSizeColumn(i);
            }

            //Save file
            var savePath = Path.Combine(Path.GetTempPath(), fileName);
            FileStream file = new FileStream(savePath, FileMode.Create);
            workbook.Write(file);
            file.Close();

            return savePath;
        }

        /// <summary>
        /// 取得屬性的顯示名稱 (字典類)
        /// Note:因類名稱規定不能重覆，這邊就不再判斷會不會加入同樣的key
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<string, DisplayItem> GetPropertyDisplayNamesMap(this Type type)
        {
            var titleListMap = new Dictionary<string, DisplayItem>();
            var propertyInfos = type.GetProperties();

            foreach (var propertyInfo in propertyInfos)
            {
                var titleName = propertyInfo.GetDisplayName();
                var cellType = CellType.STRING;
                var excelType = propertyInfo.GetCustomAttributes(typeof(ExcelDataTypeAttribute), true);
                if (excelType.Length == 1)
                {
                    var checkType = (ExcelDataTypeAttribute)excelType.FirstOrDefault();
                    if (checkType.DataType == DataType.Currency)
                    {
                        cellType = CellType.NUMERIC;
                    }
                    else if (checkType.DataType == DataType.DateTime)
                    {
                        cellType = CellType.FORMULA;
                    }
                }

                if (string.IsNullOrEmpty(titleName))
                    continue;

                titleListMap.Add(propertyInfo.Name, new DisplayItem { Name = titleName, CellType = cellType });
            }

            return titleListMap;
        }

        /// <summary>
        /// 塞資料用的 not pretty
        /// 供上面產出Excel Data 共用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataList"></param>
        /// <param name="workbook"></param>
        /// <param name="worksheet"></param>
        /// <param name="titleListMap"></param>
        private static void InsertDataValues<T>(IEnumerable<T> dataList, HSSFWorkbook workbook, ISheet worksheet, Dictionary<string, DisplayItem> titleListMap)
        {
            //Insert data values
            for (int i = 1; i < dataList.Count() + 1; i++)
            {
                var tmpRow = worksheet.CreateRow(i);
                var valueList = dataList.ElementAt(i - 1).GetPropertyValues(titleListMap);

                for (int j = 0; j < valueList.Count; j++)
                {
                    var rowCell = tmpRow.CreateCell(j);
                    var tempValue = valueList[j].Name;

                    switch (valueList[j].CellType)
                    {
                        case CellType.NUMERIC:
                            if (string.IsNullOrEmpty(tempValue) || tempValue == "------" || tempValue == "--")
                            {
                                rowCell.SetCellValue("");
                            }
                            else
                            {
                                var intValue = 0.00;
                                var flag = double.TryParse(tempValue.Replace(",", string.Empty), out intValue);
                                if (flag)
                                    rowCell.SetCellValue(intValue);
                                else
                                    rowCell.SetCellValue(tempValue);
                            }

                            var cellStyle = workbook.CreateCellStyle();
                            var format = workbook.CreateDataFormat();
                            cellStyle.DataFormat = format.GetFormat("#,##0.000");
                            rowCell.CellStyle = cellStyle;
                            break;
                        case CellType.FORMULA:
                            if (!string.IsNullOrEmpty(tempValue))
                            {
                                DateTime dateValue;
                                var flag = DateTime.TryParseExact(tempValue, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue);
                                if (flag)
                                    rowCell.SetCellValue(dateValue);
                                else
                                    rowCell.SetCellValue(tempValue);
                            }
                            else
                            {
                                rowCell.SetCellValue(tempValue);
                            }
                            var cellStyle2 = workbook.CreateCellStyle();
                            var format2 = workbook.CreateDataFormat();
                            cellStyle2.DataFormat = format2.GetFormat("yyyy/m/d");
                            rowCell.CellStyle = cellStyle2;
                            break;
                        default:
                            rowCell.SetCellValue(tempValue);
                            break;


                    }

                }

            }
        }

        /// <summary>
        /// 將T類型的公共屬性全部轉換成字符串
        /// </summary>
        /// <typeparam name="T">T類型</typeparam>
        /// <param name="data">需要轉換的對象</param>
        /// <returns>公共類型的屬性字符串集合</returns>
        private static List<string> GetPropertyValues<T>(this T data)
        {
            var properValues = new List<string>();
            var properInfos = data.GetType().GetProperties();
            foreach (var properInfoItem in properInfos)
            {
                //var value = properInfoItem.GetValue(data, null).ToString();
                var rowValue = properInfoItem.GetValue(data, null) != null ? properInfoItem.GetValue(data, null).ToString() : "";
                properValues.Add(rowValue);
            }
            return properValues;
        }

        /// <summary>
        /// 取得屬性值
        /// Note:原作者取值的方法，會有排序上的錯亂。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<RowItem> GetPropertyValues<T>(this T data, Dictionary<string, DisplayItem> columnMap)
        {
            var propertyValues = new List<RowItem>();

            var sourceData = data.GetType();
            foreach (var item in columnMap)
            {

                var propertyValue = sourceData.GetProperty(item.Key).GetValue(data, null);
                var rowValue = propertyValue != null ? propertyValue.ToString() : "";
                propertyValues.Add(new RowItem { Name = rowValue, CellType = item.Value.CellType });//排除null的情況
            }

            return propertyValues;
        }
    }

    /// <summary>
    /// 自定義欄位名稱
    /// </summary>
    public class ColNames
    {
        public string RootItem { get; set; }
        public List<string> ChildrenItem { get; set; }
        public int Length { get; set; }
        public bool HasChildrenItem()
        {
            if (this.ChildrenItem == null || this.Length == 0)
            {
                Length = 0;
                return false;
            }
            else
            {
                Length = ChildrenItem.Count();
                return true;
            }
        }
        public ColNames(string root, List<string> children)
        {
            RootItem = root;
            if (children == null)
            {
                //ChildrenItem = null;
                Length = 0;
            }
            else
            {
                ChildrenItem = children;
                Length = children.Count();
            }
        }
    }

    public class ColNamesMap
    {
        public int Length { get; set; }
        public List<ColNames> colMap { get; set; }
        public ColNamesMap(List<ColNames> map)
        {
            var lenth = 0;
            colMap = map;
            foreach (var item0 in map)
            {
                if (item0.ChildrenItem != null)
                {
                    foreach (var item1 in item0.ChildrenItem)
                    {
                        lenth += 1;
                    }
                }
                else
                {
                    lenth += 1;
                }
            }
            this.Length = lenth;
        }
    }

    /// <summary>
    /// 自訂Column物件
    /// </summary>
    public class DisplayItem
    {
        public string Name { get; set; }

        public CellType CellType { get; set; }
    }

    /// <summary>
    /// 自訂列物件
    /// </summary>
    public class RowItem
    {
        public string Name { get; set; }

        public CellType CellType { get; set; }
    }

    public class ExcelDataTypeAttribute : Attribute
    {

        public ExcelDataTypeAttribute(DataType dataType)
        {
            this.DataType = dataType;
        }
        public DataType DataType { get; set; }

    }

    public class IntToAlphabet
    {
        const int ColumnBase = 26;
        const int DigitMax = 7; // ceil(log26(Int32.Max))
        const string Digits = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static string IndexToColumn(int index)
        {
            var result = "A";
            try
            {

                if (index <= ColumnBase)
                    return Digits[index - 1].ToString();

                var sb = new StringBuilder().Append(' ', DigitMax);
                var current = index;
                var offset = DigitMax;
                while (current > 0)
                {
                    sb[--offset] = Digits[--current % ColumnBase];
                    current /= ColumnBase;
                }
                result = sb.ToString(offset, DigitMax - offset);
            }
            catch (Exception ex)
            {


            }
            return result;
        }
    }
}