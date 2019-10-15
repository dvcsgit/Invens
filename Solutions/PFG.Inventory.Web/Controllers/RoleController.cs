using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using NLog;
using PFG.Inventory.Data.Infrastructure;
using PFG.Inventory.Data.Repositories;
using PFG.Inventory.DataSource;
using PFG.Inventory.Web.Library.Enums;
using PFG.Inventory.Web.Library.Filters;
using PFG.Inventory.Web.ViewModels;
using AutoMapper.QueryableExtensions;
using PagedList;
using PagedList.Mvc;
using PFG.Inventory.Web.Library;
using PFG.Inventory.Web.Library.Extensions;


namespace PFG.Inventory.Web.Controllers
{
    /// <summary>
    /// 權限群組管理
    /// </summary>
    [SiteAuthorize]
    public class RoleController : Controller
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IPermissionOperationRepository _permissionOperationRepository;
        private readonly IUnitOfWork _unitOfWork;
        protected static Logger _logger = LogManager.GetCurrentClassLogger();

        public RoleController
            (
                IPermissionOperationRepository permissionOperationRepository,
                IUnitOfWork unitOfWork,
                IRoleRepository roleRepository,
                IPermissionRepository permissionRepository
            )
        {
            this._permissionOperationRepository = permissionOperationRepository;
            this._roleRepository = roleRepository;
            this._permissionRepository = permissionRepository;
            this._unitOfWork = unitOfWork;
        }

        [OperationCheck(EnumOperation.Query)]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 清單查詢
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [OperationCheck(EnumOperation.Query)]
        public ActionResult Query(RoleListParameter parameter)
        {
            RoleListViewModel viewModel = new RoleListViewModel() { Parameter = parameter };

            var query = _roleRepository.Query().OrderBy(x=>x.RoleID).AsQueryable();

            #region 查詢過濾資訊

            if (!string.IsNullOrEmpty(parameter.RoleName))
            {
                query = query.Where(x => x.RoleName.Contains(parameter.RoleName));
            }

            #endregion

           

            Mapper.CreateMap<Roles, RoleItem>();
            var resultList = query
                .Project().To<RoleItem>()
                .AsQueryable();

            viewModel.GridList = resultList.ToPagedList(viewModel.Parameter.PageNo, viewModel.Parameter.PageSize);
            return PartialView("_GridList", viewModel);
        }

        /// <summary>
        /// 權限群組管理 - 新增
        /// </summary>
        /// <returns></returns>
        [OperationCheck(EnumOperation.Create)]
        public ActionResult Create()
        {
            CreateOrEditRoleViewModel viewModel = new CreateOrEditRoleViewModel { };

            viewModel.AllMenu = MenuUtils.GetSubMenu();

            return View("_Create", viewModel);
        }

        [HttpGet]
        [OperationCheck(EnumOperation.Edit)]
        public ActionResult Edit(string id)
        {
            CreateOrEditRoleViewModel viewModel = new CreateOrEditRoleViewModel { SaveMode = EnumSaveMode.Update };
            var role = _roleRepository.GetById(id);
            if (role != null)
            {
                viewModel.RoleId = role.RoleID;
                viewModel.RoleName = role.RoleName;
                viewModel.Description = role.Description;
                viewModel.AllMenu = MenuUtils.GetSubMenu(id);
            }
            return View("_Edit", viewModel);
        }

        [HttpGet]
        [OperationCheck(EnumOperation.Query)]
        public ActionResult Detail(string id)
        {
            DetailRoleViewModel viewModel = new DetailRoleViewModel();
            var role = _roleRepository.GetById(id);
            if (role != null)
            {
                viewModel.RoleId = role.RoleID;
                viewModel.RoleName = role.RoleName;
                viewModel.Description = role.Description;
                viewModel.AllMenu = MenuUtils.GetSubMenu(id);
            }

            return View("_Detail", viewModel);

        }

        /// <summary>
        /// 儲存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OperationCheck(EnumOperation.Create | EnumOperation.Edit)]
        public ActionResult Save(CreateOrEditRoleViewModel model)
        {
            //valid
            //Rule1:驗證是否已經存在相同代碼 (只有新增時 檢查)
            if(model.SaveMode == EnumSaveMode.Create)
            {
                var isExist = _roleRepository.GetById(model.RoleId) != null ? true : false;
                if (isExist)
                {
                    ModelState.AddModelError("", string.Format("群組代碼{0}已存在", model.RoleId));
                }
            }

            //Rule2:檢查有沒有勾選權限
            if (model.PermissionList.Count == 0)
            {
                ModelState.AddModelError("", "請至少勾選一個權限");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userInfo = UserUtils.GetUserInfo();
                    if (model.SaveMode == EnumSaveMode.Create)
                    {
                        Roles role = new Roles
                        {
                            RoleID = model.RoleId,
                            RoleName = model.RoleName,
                            Description = model.Description,
                            CreatorAccount = userInfo.UserId,
                            DateCreated = DateTime.Now
                        };

                        //塞權限
                        foreach (var item in model.PermissionList)
                        {
                            //item is "SYS01-Delete"
                            var detailArr = item.Split('-');
                            var permissionID = detailArr[0];
                            var operationID = detailArr[1];
                            var temp = _permissionOperationRepository.Query()
                                .Where(x => x.PermissionID == permissionID && x.OperationID == operationID)
                                .FirstOrDefault();
                            role.PermissionOperations.Add(temp);
                        }

                        _roleRepository.Add(role);
                    }
                    else
                    {
                        var role = _roleRepository.GetById(model.RoleId);
                        role.RoleName = model.RoleName;
                        role.Description = model.Description;

                        //全砍再新增
                        if (role.PermissionOperations.Count > 0)
                        {
                            role.PermissionOperations.Clear();
                        }

                        //塞權限
                        foreach (var item in model.PermissionList)
                        {
                            //item is "SYS01-Delete"
                            var detailArr = item.Split('-');
                            var permissionID = detailArr[0];
                            var operationID = detailArr[1];
                            var temp = _permissionOperationRepository.Query()
                                .Where(x => x.PermissionID == permissionID && x.OperationID == operationID)
                                .FirstOrDefault();
                            role.PermissionOperations.Add(temp);
                        }
                        
                    }

                    _unitOfWork.Commit();

                    //當編輯成功更新系統的權限快取
                    if (model.SaveMode == EnumSaveMode.Update)
                    {
                        List<string> cacheKeysToDie = new List<string>();
                        // retrieve application Cache enumerator
                        var enumerator = HttpRuntime.Cache.GetEnumerator();

                        // copy all keys that currently exist in Cache
                        while (enumerator.MoveNext())
                        {
                            var tempCache = enumerator.Key.ToString();
                            if (tempCache.Contains(model.RoleId))
                                cacheKeysToDie.Add(tempCache);
                        }

                        // delete every key from cache

                        foreach (var item in cacheKeysToDie)
                        {
                            HttpRuntime.Cache.Remove(item);
                        }
                    }



                    return Json(new { success = true });

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "系統發生錯誤:" + ex.Message);
                    _logger.Error("Save - 發生錯誤:{0}", ex);
                }

            }

            return Json(new { errors = ModelState.GetErrors() });
        }


        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [OperationCheck(EnumOperation.Delete)]
        public ActionResult Delete(string id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var oldItem = _roleRepository.GetById(id);
                    if (oldItem != null)
                    {
                        if (oldItem.Users.Count > 0)
                        {
                            oldItem.Users.Clear();
                        }
                        _roleRepository.Delete(oldItem);
                        _unitOfWork.Commit();
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ModelState.AddModelError("", "資料不存在,可能已經被刪除!?");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "系統發生錯誤");
                    _logger.Error("Delete - 發生錯誤:{0}", ex);
                }

            }

            return Json(new { errors = ModelState.GetErrors() }, JsonRequestBehavior.AllowGet);
        }
    }


}