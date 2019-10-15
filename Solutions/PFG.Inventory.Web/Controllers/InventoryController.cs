using PFG.Inventory.Web.Library.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PFG.Inventory.DataSource;
using PFG.Inventory.Web.Library.Enums;
using PFG.Inventory.Web.ViewModels;
using PagedList;
using System.ComponentModel;
using PFG.Inventory.Web.Library;
using PFG.Inventory.Web.Library.Extensions;
using System.Data.Entity;

namespace PFG.Inventory.Web.Controllers
{
    [SiteAuthorize]
    public class InventoryController : Controller
    {
        // GET: Inventory
        [OperationCheck(EnumOperation.Query)]
        public ActionResult Index()
        {
            return View();
        }

        [OperationCheck(EnumOperation.Export | EnumOperation.Query)]
        public ActionResult Query(InventoryParameters parameters,[DefaultValue(false)]bool isExport)
        {
            var viewModel = new InventoryListViewModels()
            {
                Parameters = parameters
            };
            using (PFGWarehouseEntities db = new PFGWarehouseEntities())
            {
                
                //其中WarehouseInfo的值是前端畫面select（DropDownListFor）選擇的Value值，其顯示的值是ID
                var query = db.Inventory.Where(x => x.WarehouseID == parameters.WarehouseInfo&&x.StatusCode=="Y");
                if(!string.IsNullOrEmpty(parameters.Location.ToString()))
                {
                    query = query.Where(x => x.Location == parameters.Location);
                }
                if (!string.IsNullOrEmpty(parameters.ProductCode))
                {
                    query = query.Where(x => x.ProductCode == parameters.ProductCode);
                }
                if(!string.IsNullOrEmpty(parameters.Class))
                {
                    query = query.Where(x => x.Class == parameters.Class);
                }
                if (parameters.IsPointer)
                {
                    if (string.IsNullOrEmpty(parameters.EnterFlag))
                    {
                        query = query.Where(x => !string.IsNullOrEmpty(x.Remark));
                    }
                    else
                    {
                        query = query.Where(x => !string.IsNullOrEmpty(x.Remark) && x.Remark.Contains(parameters.EnterFlag));
                    }
                }
                if (!parameters.IsPointer)
                {
                    if (!string.IsNullOrEmpty(parameters.EnterFlag))
                    {
                        query = query.Where(x => x.Remark.Contains(parameters.EnterFlag));
                    }
                }

                var resultList = query.Select(x => new InventoryViewModels()
                {
                    WarehouseInfo=x.WarehouseID,
                    Location = x.Location,
                    ProductCode = x.ProductCode,
                    BoxNumber = x.BoxNumber,
                    Class = x.Class,
                    NetWeight = x.NetWeight,
                    CrossWeight = x.GrossWeight,
                    EnterFlag=x.Remark,
                    OldBoxNumber=x.BoxNumber
                }).AsQueryable();
                if (!string.IsNullOrEmpty(parameters.WarehouseInfo))
                {
                    decimal sumNetWeight = 0;
                    decimal sumGrossWeight = 0;
                    IPagedList<InventoryViewModels> gridList=null ;
                    if (parameters.IsPointer)
                    {
                        gridList = resultList.OrderBy(x => x.EnterFlag).ToPagedList(parameters.PageNo, parameters.PageSize);
                    }
                    else
                    {
                        gridList = resultList.OrderBy(x => x.Location).ToPagedList(parameters.PageNo, parameters.PageSize);
                    }
                    

                    //var index = 0;
                    foreach(var item in gridList)
                    {
                        //index += 1;
                        sumNetWeight += Convert.ToDecimal(item.NetWeight);
                        sumGrossWeight += Convert.ToDecimal(item.CrossWeight);
                        //item.FlagIndex = index;
                    }
                    var capacity = from inventory in db.Inventory
                                   join basicSettingCapacity in db.BasicSettingCapacity
                                   on inventory.ProductCode.Substring(0, 2) equals basicSettingCapacity.ProductCode.Trim()
                                   where inventory.WarehouseID==parameters.WarehouseInfo &&inventory.StatusCode=="Y"
                                   select new CapacityViewModels() { Capacity = basicSettingCapacity.CapacityProduct,ProductCode=basicSettingCapacity.ProductCode.Trim() };
                                   //into g
                                   //from m in g
                                   //where inventory.WarehouseID == parameters.WarehouseInfo
                                   //select new CapacityViewModels() { Capacity = m.CapacityProduct,ProductCode=m.ProductCode.Trim() };
                    var bsCapacity = db.BasicSettingCapacity.Select(x => new
                    {
                        Code = x.ProductCode.Trim(),
                        Capacity = x.CapacityProduct
                    });
                    //var capacityProduct = from basicCapacity in db.BasicSettingCapacity
                    //                      join inventory in db.Inventory
                    //                      on basicCapacity.ProductCode equals inventory.ProductCode.Substring(0, 2)
                    //                      where inventory.WarehouseID == parameters.WarehouseInfo
                    //                      select new BasicSettingCapacityViewModels() { 
                    //                      CapacityProduct=basicCapacity.CapacityProduct,
                    //                      ProductCode=basicCapacity.ProductCode
                    //                      };
                    double total = 0;
                    var warehouseCapacity = db.BasicSettingWarehouse.Single(x => x.WarehouseID == parameters.WarehouseInfo).Capacity;
                    foreach(var item in bsCapacity)
                    {
                        //if (item.Code == "TK")
                        //{
                            //var count = db.Inventory.Where(x => x.ProductCode.Substring(0, 2) == item.Code).Count();
                        var count = capacity.Where(x => x.ProductCode == item.Code).Count();
                        if (count > 0)
                        {
                            total += Math.Round((double)(count * 4 / (int)item.Capacity),4);
                        }
                    }
                    decimal percentage = ((decimal)total / (decimal)warehouseCapacity)*(decimal)100;
                    
                    //var total = capacity.Sum(x => x.Capacity);
                    //var total = (capacity.Count() * 4)/3;
                    //var warehouseCapacity = db.BasicSettingWarehouse.Single(x => x.WarehouseID == parameters.WarehouseInfo).Capacity;
                   // var percentage = (total / warehouseCapacity) * 100;
                    //if (percentage==null)
                    //{
                    //    percentage = 0;
                    //}
                    viewModel.SumNetWeight = sumNetWeight;
                    viewModel.SumGrossWeight = sumGrossWeight;
                    viewModel.CapacityPercentage = Math.Round(double.Parse(percentage.ToString()),2) + "%";
                    viewModel.GridList = gridList;
                }
                else
                {
                    viewModel.CapacityPercentage = "0%";
                }
                if (isExport)
                {
                    //List<int> outCol = new List<int>() { 8};
                    //List<string> titles = new List<string>() { "外倉","庫位","箱號","產品批號","等級","淨重","毛重","輸入註記"};
                    List<int> outCol = new List<int>() { 8 };
                    var desFilePath = resultList.ToList().ExportExcel("外倉("+parameters.WarehouseInfo+")庫存信息.xls", "外倉庫存信息",parameters.WarehouseInfo,null,outCol);
                    var url = Url.Action("Download", "Utils", new { @fullFilePath = desFilePath });
                    return Json(new { success = true, url = url }); 
                }
               
                
            }
            return PartialView("_GridList", viewModel);
        }

        [HttpPost]
        public ActionResult Save(List<InventoryViewModels> GridList)
        {

            //Dictionary<string, string> result = new Dictionary<string, string>();
            //for (int i = 0; i < value.Count(); i++)
            //{
            //    //if(text.ElementAt(i).Trim().Length==0)
            //    //{
            //    //    continue;
            //    //}
            //    result.Add(value.ElementAt(i), text.ElementAt(i));
            //}
            //var db = new PFGWarehouseEntities();
            using (PFGWarehouseEntities db = new PFGWarehouseEntities())
            {
                var userModel = User.GetMVCUser();
                var info = new List<string>();
                //var bxInfo = new List<string>();
                foreach (var item in GridList)
                {
                    //統計看是否存在這一筆出口案號，如果存在就擋掉
                    var cntBoxNumber = db.Inventory.Where(x => x.BoxNumber == item.BoxNumber).Count();
                    if(string.IsNullOrEmpty(item.BoxNumber))
                    {
                        info.Add(item.BoxNumber+"箱號不能為空！");
                        continue;
                    }
                    if(string.IsNullOrEmpty(item.Class.Trim()))
                    {
                        info.Add(item.Class + "等級不能為空！");
                    }
                    if(item.BoxNumber.Length>7)
                    {

                        info.Add(item.BoxNumber+"箱號不能超過7個字符！");
                        continue;
                    }
                    if(item.BoxNumber!=item.OldBoxNumber&&cntBoxNumber>0)
                    {
                        info.Add(item.BoxNumber+"箱號已存在！");
                        continue;
                    }
                    if(string.IsNullOrEmpty(item.ProductCode))
                    {
                        info.Add("箱號" + item.BoxNumber + "之產品批號為必要項！");
                        continue;
                    }
                    if(!string.IsNullOrEmpty(item.EnterFlag)&&item.ProductCode.Length>10)
                    {
                        info.Add("箱號" + item.BoxNumber + "之產品批號字符不能超過10個字符！");
                        continue;
                    }
                    if(!string.IsNullOrEmpty(item.EnterFlag)&&item.EnterFlag.Length>10)
                    {
                        info.Add("箱號"+item.BoxNumber+"之輸入註記不能超過10個字符！");
                        continue;
                    }
                    if (string.IsNullOrEmpty(item.NetWeight))
                    {
                        info.Add("箱號" + item.BoxNumber + "之淨重為必要項！");
                        continue;
                    }
                    if (string.IsNullOrEmpty(item.CrossWeight))
                    {
                        info.Add("箱號" + item.BoxNumber + "之毛重為必要項！");
                        continue;
                    }
                    int result = 0;
                    if (!string.IsNullOrEmpty(item.NetWeight)&&!int.TryParse(item.NetWeight,out result))
                    {
                        info.Add("箱號" + item.BoxNumber + "之淨重須為整數！");
                        continue;
                    }
                    if (!string.IsNullOrEmpty(item.CrossWeight) && !int.TryParse(item.CrossWeight, out result))
                    {
                        info.Add("箱號" + item.BoxNumber + "之淨重須為整數！");
                        continue;
                    }
                    //var saveObject = db.Inventory.Single(x => x.BoxNumber == item.OldBoxNumber);
                    //saveObject.BoxNumber = item.BoxNumber;
                    //db.Entry<Inventory.DataSource.Inventory>(saveObject).State = EntityState.Modified;
                    //db.SaveChanges();
                    if (item.OldBoxNumber != item.BoxNumber)
                    {
                        //不能直接使用EF中的方法更新主鍵列，需要用執行sql的方式更新。
                        string sql = @"  update [PFGWarehouse].[dbo].[Inventory]
  set BoxNumber='" + item.BoxNumber + "' where BoxNumber='" + item.OldBoxNumber + "'";
                        db.Database.ExecuteSqlCommand(sql);
                        db.SaveChanges();
                    }
                    var newObject = db.Inventory.Single(x => x.BoxNumber == item.BoxNumber);
                    newObject.Remark = item.EnterFlag == "" || item.EnterFlag == null ? string.Empty : item.EnterFlag;
                    newObject.ModifierAccount = userModel.UserId;
                    newObject.DateModified = DateTime.Now;
                    //saveObject.BoxNumber = item.EnterFlag;
                    newObject.ProductCode = item.ProductCode;
                    newObject.Class = item.Class;
                    newObject.NetWeight = item.NetWeight;
                    newObject.GrossWeight = item.CrossWeight;
                    db.Entry<Inventory.DataSource.Inventory>(newObject).State = EntityState.Modified;
                    db.SaveChanges();
                }
                //db.SaveChanges();
                if (info.Count() > 0)
                {
                    var infoText = "以下箱號輸入信息有誤：<br/>";
                    foreach (var item in info)
                    {
                        infoText += item + "<br/>";
                    }
                    infoText += "請重新輸入！";
                    return Json(new { errors = infoText });
                }
                return Json(new { success = true });
            }
        }
    }
}