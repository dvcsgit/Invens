using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using NLog;
using PFG.Inventory.Data.Repositories;
using PFG.Inventory.DataSource;
using PFG.Inventory.Web.Library.Filters;
using PFG.Inventory.Web.Models;
using PFG.Inventory.Web.ViewModels;
using AutoMapper.QueryableExtensions;
using PagedList;

namespace PFG.Inventory.Web.Controllers
{
    [SiteAuthorize]
    public class UploadLogController : Controller
    {
        protected static Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IUploadLogRepository _uploadLogRepository;

        public UploadLogController(IUploadLogRepository uploadLogRepository)
        {
            this._uploadLogRepository = uploadLogRepository;
        }

        /// <summary>
        /// 首頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Query(UploadLogListParameter parameter)
        {
            UploadLogListViewModel viewModel = new UploadLogListViewModel() { Parameter = parameter };
            var query = _uploadLogRepository.Query();

            #region 查詢過濾資訊

            if (!string.IsNullOrEmpty(parameter.Account))
            {
                query = query.Where(x => x.Account.Contains(parameter.Account));
            }

            #endregion

            #region 排序處理
            switch (viewModel.Parameter.SortingField)
            {
                case UploadLogSortingField.Account:
                    if (viewModel.Parameter.SortingDirection == SortingDirection.Desc)
                        query = query.OrderByDescending(x => x.Account);
                    else
                        query = query.OrderBy(x => x.Account);
                    break;
                case UploadLogSortingField.DataUpload:
                    if (viewModel.Parameter.SortingDirection == SortingDirection.Desc)
                        query = query.OrderByDescending(x => x.DataUpload);
                    else
                        query = query.OrderBy(x => x.DataUpload);
                    break;
                default:
                    viewModel.Parameter.SortingField = UploadLogSortingField.None;
                    viewModel.Parameter.SortingDirection = SortingDirection.None;
                    query = query.OrderByDescending(x => x.DataUpload);
                    break;
            }
            #endregion

            Mapper.CreateMap<UploadLog, UploadLogItem>();
            var resultList = query
                .Project().To<UploadLogItem>()
                .AsQueryable();

            viewModel.GridList = resultList.ToPagedList(viewModel.Parameter.PageNo, viewModel.Parameter.PageSize);
            return PartialView("_GridList", viewModel);
        }
    }
}