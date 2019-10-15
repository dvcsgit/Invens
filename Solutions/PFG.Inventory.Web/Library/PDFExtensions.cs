using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using PFG.Inventory.Web.ViewModels;
using PFG.Inventory.Web.Models;

namespace PFG.Inventory.Web.Library
{
    public static class PDFExtensions
    {
        public static Dictionary<string,IEnumerable<OutBoundInventoryViewModels>> GroupByProductCode(this IEnumerable<OutBoundInventoryViewModels> source,List<OutBoundInventoryQuery> l)
        {
            var productCode = source.Select(x => x.ProductCode).Distinct();
            Dictionary<string, IEnumerable<OutBoundInventoryViewModels>> dicList = new Dictionary<string, IEnumerable<OutBoundInventoryViewModels>>();
            foreach(string code in productCode)
            {
                var productClass = source.Where(x => x.ProductCode == code).Select(x => x.Class).Distinct();
                
                foreach (string itemClass in productClass)
                {
                    var itemClassTrim = itemClass.Trim();
                    var vhnos = l.Where(x => x.PdId == code && x.Gd == itemClassTrim).FirstOrDefault();
                    IEnumerable<OutBoundInventoryViewModels> result = source.Where(x => x.ProductCode == code && x.Class == itemClassTrim)
                        .Select(x => new OutBoundInventoryViewModels()
                        {
                            BoxNumber = x.BoxNumber,
                            CarNo = x.CarNo,
                            Class = x.Class,
                            DateUpload = x.DateUpload,
                            GrossWeight = x.GrossWeight,
                            No = x.No,
                            Location = x.Location,
                            NetWeight = x.NetWeight,
                            ProductCode = x.ProductCode,
                            WarehouseID = x.WarehouseID,
                            UploadAccount = x.UploadAccount,
                            Vhno=vhnos.VhNo
                            //Vhno=""
                        });
                    var convertVhno =vhnos!=null&&!string.IsNullOrEmpty(vhnos.VhNo) ?vhnos.VhNo.Split('_')[1]:"";
                    //var convertVhno = "";
                    dicList.Add(convertVhno+"."+code+"."+itemClass, result);
                }
            }
            //var dic = dicList.OrderBy(x=>x.Key);
            return dicList.OrderBy(x=>x.Key).ToDictionary(x=>x.Key,x=>x.Value);
 
        }

        public static string ExportPDF(this Dictionary<string,IEnumerable< OutBoundInventoryViewModels>> source,string fileName)
        {
            return Show(fileName, source);

        }
        private static void SetValue(this PdfPTable table, string value,BaseFont bfChinese)
        {
            PdfPCell valueCell = new PdfPCell(new Phrase(value, new Font(bfChinese, 13)));
            valueCell.BorderWidth = 0;
            table.AddCell(valueCell);
        }

        private static void SetAlignValue(this PdfPTable table,string value,BaseFont bfChinese)
        {
            PdfPCell valueCell = new PdfPCell(new Phrase(value, new Font(bfChinese, 13)));
            valueCell.BorderWidth = 0;
            valueCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            table.AddCell(valueCell);
        }

        private static string Show(string fileName,Dictionary<string,IEnumerable< OutBoundInventoryViewModels>> source)
        {
            Document pdf = new Document(PageSize.A4, 5, 5, 20, 20);

            

            //下載保存
            var savePath = Path.Combine(Path.GetTempPath(), fileName);
            FileStream fs = new FileStream(savePath, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(pdf, fs);
            pdf.Open();
            //設置繁體中文字體
            BaseFont bfChinese = BaseFont.CreateFont(@"C:\Windows\Fonts\kaiu.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            foreach (var item in source)
            {

                //項次
                int index = 0;
                //毛重
                int grossWeight = 0;
                //淨重
                int netWeight = 0;
                //string codeClass = item.Value.ElementAt(0).Class;
                string code = item.Key.Split('.').ElementAt(1);
                string codeClass = item.Key.Split('.').ElementAt(2);
                string carNo = item.Value.ElementAt(0).CarNo;
                string Warehouse = item.Value.ElementAt(0).WarehouseID;
                string dateUpload = item.Value.ElementAt(0).Vhno;
                foreach (var itemSource in item.Value)
                {
                    index += 1;
                    string itemIndex = index.ToString();
                    
                    grossWeight += int.Parse(itemSource.GrossWeight);
                    netWeight += int.Parse(itemSource.NetWeight);
                }

                pdf.NewPage();
                string[] codeValues = { code, codeClass, netWeight.ToString(), grossWeight.ToString(), index.ToString() };
                PdfPTable allTable = new PdfPTable(new float[] { 8, 3 });


                allTable.SplitLate = false;
                allTable.SplitRows = true;

                PdfPCell titleCell = new PdfPCell(SetPhrase(bfChinese, "交 運 明 細 表", 16, Font.UNDERLINE)).SetNonborder();
                titleCell.FixedHeight = 30;
                titleCell.HorizontalAlignment = Element.ALIGN_CENTER;
                //titleCell.Colspan = 2;
                //titleCell.PaddingRight = 30;
                allTable.AddCell(titleCell);

                PdfPCell titleDateCell = new PdfPCell(SetPhrase(bfChinese, "出表日："+DateTime.Now.ToChineseDate(), 10, Font.NORMAL)).SetNonborder();
                titleDateCell.PaddingTop = 7;
                titleDateCell.HorizontalAlignment = Element.ALIGN_LEFT;
                allTable.AddCell(titleDateCell);

                PdfPTable infoTable = SetTableInfoList(bfChinese, carNo, Warehouse,dateUpload);
                PdfPCell ListInfoCell = new PdfPCell(infoTable).SetNonborder();
                allTable.AddCell(ListInfoCell);

                PdfPCell ImgCell = new PdfPCell(SetTableImg(codeValues)).SetNonborder();
                ImgCell.Rowspan = 3;
                allTable.AddCell(ImgCell);

                PdfPCell dataCell = new PdfPCell(SetTableData(item.Value, bfChinese)).SetNonborder();
                dataCell.PaddingTop = 10;
                allTable.AddCell(dataCell);

                PdfPCell footCell = new PdfPCell(SetTableFooter(bfChinese)).SetNonborder();
                allTable.AddCell(footCell);
                pdf.Add(allTable);

                
            }

            //string[] codes = { "3", "5", "9M" };
            //List<Image> imgs = GetAllImg(codes);
            //foreach(var item in imgs)
            //{
            //    pdf.Add(item);
            //    pdf.Add(new Phrase("        "));
            //}

            pdf.Close();
            fs.Close();
            return savePath;
        }

        private static PdfPTable SetTableData(IEnumerable<OutBoundInventoryViewModels> list,BaseFont bf)
        {
            PdfPTable invTable = new PdfPTable(6);
            //超過一頁的可以跨頁
            invTable.SplitLate = true;
            invTable.SplitRows = true;

            string[] invt = {"項 次","箱(車)號","產品代號","等 級","毛 重","淨 重" };
            foreach(var item in invt)
            {
                PdfPCell pCell = new PdfPCell(SetPhrase(bf, item, 13, Font.UNDERLINE)).SetNonborder();
                if (item == "毛 重" || item == "淨 重")
                {
                    pCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                }
                invTable.AddCell(pCell);
            }
            //項次
            int index = 0;
            //毛重
            int grossWeight=0;
            //淨重
            int netWeight=0;
            foreach (var item in list)
            {
                index+=1;
                string itemIndex=index.ToString();
                invTable.SetValue(itemIndex, bf);
                invTable.SetValue(item.BoxNumber, bf);
                invTable.SetValue(item.ProductCode, bf);
                invTable.SetValue(item.Class,bf);
                //invTable.SetValue(item.GrossWeight, bf);
                invTable.SetAlignValue(item.GrossWeight, bf);
                //invTable.SetValue(item.NetWeight, bf);
                invTable.SetAlignValue(item.NetWeight, bf);
                grossWeight+=int.Parse(item.GrossWeight);
                netWeight+=int.Parse(item.NetWeight);
            }
            invTable.SetValue("-----", bf);
            invTable.SetValue("", bf);
            invTable.SetValue("", bf);
            invTable.SetValue("", bf);
            invTable.SetAlignValue("-----", bf);
            invTable.SetAlignValue("-----", bf);
            invTable.SetValue(index.ToString(), bf);
            invTable.SetValue("", bf);
            invTable.SetValue("", bf);
            invTable.SetValue("", bf);
            //invTable.SetValue(grossWeight.ToString(),bf);
            invTable.SetAlignValue(grossWeight.ToString(), bf);
            //invTable.SetValue(netWeight.ToString(), bf);
            invTable.SetAlignValue(netWeight.ToString(), bf);
            return invTable;
        }

        private static PdfPTable SetTableFooter(BaseFont bf)
        {
            PdfPTable footTable = new PdfPTable(2);
            
            string[] footStr = { "成品課確認簽名：","","裝櫃人員確認簽名：","裝貨數量共：      板"};
            int i = 0;
            foreach(var item in footStr)
            {
                PdfPCell valueCell = new PdfPCell(new Phrase(item, new Font(bf, 12)));
                if (i < 2)
                {
                    valueCell.FixedHeight = 40;
                    i += 1;
                    valueCell.VerticalAlignment = Element.ALIGN_BOTTOM;
                    valueCell.HorizontalAlignment = Element.ALIGN_CENTER;
                }
                if(i==2)
                {
                    valueCell.VerticalAlignment = Element.ALIGN_TOP;
                    valueCell.HorizontalAlignment = Element.ALIGN_CENTER;
                }

                valueCell.BorderWidth = 0;
                footTable.AddCell(valueCell);
            }
            return footTable;
        }

        private static PdfPTable SetTableImg(string[] values)
        {
            PdfPTable imgTable = new PdfPTable(1);
            List<Image> imgs = GetAllImg(values);
            foreach(var item in imgs)
            {
                PdfPCell cell = new PdfPCell(item).SetNonborder();
                cell.FixedHeight = 90;
                cell.PaddingLeft = 50;
                cell.PaddingBottom = 40;
                imgTable.AddCell(cell);
            }
            return imgTable;
        }

        private static List<Image> GetAllImg(string[] values)
        {
            List<Image> list = new List<Image>();
            string[] title={"產品代號","等級","淨重","毛重","件數"};
            int index=0;
            foreach (var item in values)
            {
                Code39 code39 = new Code39();
                code39.Height = 40;
                code39.Magnify = 0;
                System.Drawing.Image codeImage = code39.GetCodeImage(item, Code39.Code39Model.Code39Normal, true,title[index]);
                var savePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".jpg");
                FileStream fs = new FileStream(savePath, FileMode.Create);
                codeImage.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                Image img = Image.GetInstance(codeImage,BaseColor.WHITE);
                
                list.Add(img);
                fs.Close();
                index += 1;
            }

            return list;
        }

        private static PdfPTable SetTableInfoList(BaseFont bf,string carNo,string warehouseId,string dateUpload)
        {
            PdfPTable infoTable = new PdfPTable(3);
            List<string> infoList = GetAllInfoList(carNo,warehouseId,dateUpload);
            foreach(var item in infoList)
            {
                PdfPCell cell = new PdfPCell(SetPhrase(bf, item, 12, Font.NORMAL)).SetNonborder();
                //cell.Width = 200;
                infoTable.AddCell(cell);
            }

            return infoTable;
        }

        

        private static List<string> GetAllInfoList(string carNo,string warehouseId,string dateUpload)
        {
            List<string> infos = new List<string>()
            {
                "客戶名稱：",
                "訂單名稱：",
                "發貨庫："+warehouseId,
                "品名規格：",
                "貨(車)櫃編號："+carNo,
                "交運單編號"+dateUpload
            };
            return infos;
        }

        private static Phrase SetPhrase(BaseFont bfChinese,string text,int size,int fontStyle)
        {
            //BaseFont bfChinese = BaseFont.CreateFont(@"C:\Windows\Fonts\kaiu.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Phrase titleParagraph = new Phrase(text, new Font(bfChinese, size, fontStyle));
            return titleParagraph;
        }

        private static PdfPCell SetNonborder(this PdfPCell cell) 
        {
            cell.BorderWidth = 0;
            return cell;
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

        public static string GetExpno(DateTime dateUpload)
        {
            string result = dateUpload.ToString("yyyy-MM-dd HH:mm:ss").Replace("-", "").Replace(":", "").Replace(" ", "").Substring(2, 12);
            return result;

        }

    }
}