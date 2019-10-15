using System;
using System.ComponentModel;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using NLog;
using PagedList;
using PFG.Inventory.Data.Infrastructure;
using PFG.Inventory.Data.Repositories;
using PFG.Inventory.DataSource;
using PFG.Inventory.Web.Library;
using PFG.Inventory.Web.Library.Enums;
using PFG.Inventory.Web.Library.Extensions;
using PFG.Inventory.Web.Library.Filters;
using PFG.Inventory.Web.Models;
using PFG.Inventory.Web.ViewModels;

namespace PFG.Inventory.Web.Controllers
{
    /// <summary>
    /// 使用者管理
    /// </summary>
    [SiteAuthorize]
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        protected static Logger _logger = LogManager.GetCurrentClassLogger();

        public AccountController
            (
                IUserRepository userRepository,
                IRoleRepository roleRepository,
                IUnitOfWork unitOfWork,
                IDatabaseFactory databaseFactory
            )
        {

            this._userRepository = userRepository;
            this._roleRepository = roleRepository;
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
        public ActionResult Query(AccountListParameter parameter)
        {
            AccountListViewModel viewModel = new AccountListViewModel() { Parameter = parameter };
            var query = _userRepository.Query();

            #region 查詢過濾資訊

            if (!string.IsNullOrEmpty(parameter.Account))
            {
                query = query.Where(x => x.Account.Contains(parameter.Account));
            }

            #endregion

            #region 排序處理
            switch (viewModel.Parameter.SortingField)
            {
                case AccountSortingField.DateCreated:
                    if (viewModel.Parameter.SortingDirection == SortingDirection.Desc)
                        query = query.OrderByDescending(x => x.DateCreated);
                    else
                        query = query.OrderBy(x => x.DateCreated);
                    break;
                case AccountSortingField.DateLastLogin:
                    if (viewModel.Parameter.SortingDirection == SortingDirection.Desc)
                        query = query.OrderByDescending(x => x.DateLastLogin);
                    else
                        query = query.OrderBy(x => x.DateLastLogin);
                    break;
                default:
                    viewModel.Parameter.SortingField = AccountSortingField.None;
                    viewModel.Parameter.SortingDirection = SortingDirection.None;
                    query = query.OrderByDescending(x => x.DateCreated);
                    break;
            }
            #endregion

            Mapper.CreateMap<Users, UserItem>();
            var resultList = query
                .Project().To<UserItem>()
                .AsQueryable();

            viewModel.GridList = resultList.ToPagedList(viewModel.Parameter.PageNo, viewModel.Parameter.PageSize);
            return PartialView("_GridList", viewModel);
        }

        /// <summary>
        /// 帳號管理 - 新增
        /// </summary>
        /// <returns></returns>
        [OperationCheck(EnumOperation.Create)]
        public ActionResult Create()
        {
            CreateOrEditAccountViewModel viewModel = new CreateOrEditAccountViewModel { };
            return View("_Create", viewModel);
        }

        /// <summary>
        /// 帳號管理 - 詳細內容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [OperationCheck(EnumOperation.Query)]
        public ActionResult Detail(string id)
        {
            DetailAccountViewModel viewModel = new DetailAccountViewModel();
            var user = _userRepository.GetById(id);
            Mapper.CreateMap<Users, DetailAccountViewModel>();
            viewModel = Mapper.Map<Users, DetailAccountViewModel>(user);
            viewModel.UserRoles = user.Roles.Select(x=>x.RoleID).ToList();
            return View("_Detail", viewModel);

        }

        /// <summary>
        /// 帳號管理 - 修改
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [OperationCheck(EnumOperation.Edit)]
        public ActionResult Edit(string id)
        {
            CreateOrEditAccountViewModel viewModel = new CreateOrEditAccountViewModel { };
            var user = _userRepository.GetById(id);
            Mapper.CreateMap<Users, CreateOrEditAccountViewModel>();
            viewModel = Mapper.Map<Users, CreateOrEditAccountViewModel>(user);
            viewModel.SaveMode = EnumSaveMode.Update;
            viewModel.UserRoles = user.Roles.Select(x => x.RoleID).ToList();
            return View("_Edit", viewModel);
        }

        /// <summary>
        /// 帳號管理 - 刪除
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
                    var oldItem = _userRepository.GetById(id);
                    if (oldItem != null)
                    {
                        if (oldItem.Roles.Count > 0)
                        {
                            oldItem.Roles.Clear();
                        }
                        _userRepository.Delete(oldItem);
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

            return Json(new { errors = ModelState.GetErrors() } , JsonRequestBehavior.AllowGet );
        }

        /// <summary>
        /// 帳號管理 - 儲存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OperationCheck(EnumOperation.Create | EnumOperation.Edit)]
        public ActionResult Save(CreateOrEditAccountViewModel model)
        {
            var userInfo = UserUtils.GetUserInfo();
            
            if (ModelState.IsValid)
            {
                try
                {
                    
                    if (model.SaveMode == EnumSaveMode.Create)
                    {
                        //自訂驗證
                        if (_userRepository.Query().Where(x => x.Account == model.Account).Any())
                        {
                            ModelState.AddModelError("", "已有相同的帳號");
                        }

                        if (ModelState.IsValid)
                        {
                            Mapper.CreateMap<CreateOrEditAccountViewModel, Users>();
                            var newItem = Mapper.Map<CreateOrEditAccountViewModel, Users>(model);
                            newItem.CreatorAccount = userInfo.UserId;
                            newItem.DateCreated = DateTime.Now;

                            //新增權限
                            var role = _roleRepository.Query().Where(x => model.UserRoles.Contains(x.RoleID)).ToList();
                            newItem.Roles = role;

                            _userRepository.Add(newItem);
                            _unitOfWork.Commit();
                            return Json(new { success = true });
                        }
                        
                    }
                    else
                    {
                        var oldItem = _userRepository.GetById(model.Account);
                        if (oldItem != null)
                        {
                            oldItem.PasswordHash = model.ConfirmPassword;
                            oldItem.Name = model.Name;
                            oldItem.Email = model.Email;
                            oldItem.ModifierAccount = userInfo.UserId;
                            oldItem.DateModified = DateTime.Now;

                            //權限 全砍再新增
                            if (oldItem.Roles.Count > 0)
                            {
                                oldItem.Roles.Clear();
                            }

                            if (model.UserRoles != null)
                            {
                                var role = _roleRepository.Query().Where(x => model.UserRoles.Contains(x.RoleID)).ToList();
                                oldItem.Roles = role;
                            }

                            _userRepository.Update(oldItem);
                            _unitOfWork.Commit();
                            return Json(new { success = true });
                        }
                        else
                        {
                            ModelState.AddModelError("", "此帳號不存在");
                        }
                        
                    }
                    
                    

                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            _logger.Error("Execute Error Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                    ModelState.AddModelError("", "系統發生錯誤");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "系統發生錯誤");
                    _logger.Error("Save - 發生錯誤:{0}", ex);
                }

            }

            return Json(new { errors = ModelState.GetErrors() });

        }

        

    }
}
