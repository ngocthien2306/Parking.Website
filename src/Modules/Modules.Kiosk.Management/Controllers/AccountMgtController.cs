using AutoMapper;
using InfrastructureCore;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Kiosk.Management.Repositories.IRepositories;
using Modules.Pleiger.CommonModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modules.Kiosk.Management.Controllers
{
    public class AccountMgtController : BaseController
    {

        private readonly IMapper mapper;
        private readonly IAccessMenuService _accessMenuService;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IUserService _userService;
        private readonly IKIOAccountRepository _accountRepository;
        public AccountMgtController(IMapper mapper, IKIOAccountRepository accountRepository,
            IAccessMenuService accessMenuService, IHttpContextAccessor contextAccessor, IUserService userService) : base(contextAccessor)
        {
            this.mapper = mapper;
            this._accessMenuService = accessMenuService;
            this.contextAccessor = contextAccessor;
            this._userService = userService;
            this._accountRepository = accountRepository;
        }

        #region View

        public ActionResult Index()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var curUrlTemp = (Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            var curMenu = CurrentUser != null ? CurrentUser.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;

            var ListButtonPermissionByUser = _accessMenuService.GetButtonPermissionByUser(CurrentUser.SiteID, curMenu.MenuID, CurrentUser.UserCode);
            if (ListButtonPermissionByUser.Count > 0)
            {
                ViewBag.CREATE_YN = ListButtonPermissionByUser[0].CREATE_YN;
                ViewBag.SAVE_YN = ListButtonPermissionByUser[0].SAVE_YN;
                ViewBag.EDIT_YN = ListButtonPermissionByUser[0].EDIT_YN;
                ViewBag.DELETE_YN = ListButtonPermissionByUser[0].DELETE_YN;
                ViewBag.SEARCH_YN = ListButtonPermissionByUser[0].SEARCH_YN;
                ViewBag.UPLOAD_FILE_YN = ListButtonPermissionByUser[0].UPLOAD_FILE_YN;
            }
            ViewBag.MenuId = curMenu != null ? curMenu.MenuID : 0;
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }

        public IActionResult ShowDetailAccountMgt(string storeNo, string userId, string viewbagIndex, int menuParent)
        {
            ViewBag.SAVE_YN = false;
            ViewBag.DELETE_YN = false;
            var ListButtonPermissionByUser = _accessMenuService.GetButtonPermissionByUser(CurrentUser.SiteID, menuParent, CurrentUser.UserCode);
            if (ListButtonPermissionByUser.Count > 0)
            {
                ViewBag.SAVE_YN = ListButtonPermissionByUser[0].SAVE_YN;
                ViewBag.DELETE_YN = ListButtonPermissionByUser[0].DELETE_YN;
            }

            ViewBag.Thread = Guid.NewGuid().ToString("N");
            //var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID);
            //ViewBag.UserType = checkUserLogin.SystemUserType;
            ViewBag.Index = viewbagIndex;
            ViewBag.UserName = CurrentUser.UserName;
            ViewBag.UserId = CurrentUser.UserID;
            ViewBag.UId = userId;
            ViewBag.StoreNo = storeNo;
            //var users = new List<KIO_UseRegisteredStore>();
            //if (userId != null)
            //{
            //    users = _accountRepository.GetUserRegisteredStore(storeNo, userId);
            //}
            return PartialView("ShowDetailAccountMgt");

        }
        #endregion

        #region Get Data
        [HttpGet]
        public ActionResult<List<KIO_AccountMgt>> GetAccountMgt(string userId)
        {
            return Json(_accountRepository.GetAccountMgt(userId));
        }
        [HttpGet]
        public ActionResult<List<KIO_AccountMgt>> GetUserRegisteredStore(string storeNo, string userId)
        {
            return Json(_accountRepository.GetUserRegisteredStore(storeNo, userId));
        }
        #endregion

        #region Create - Update - Delete
        public ActionResult<Result> SaveAccountMgt(string userId, bool status)
        {
            return Json(_accountRepository.SaveAccountMgt(userId, status));
        }
        public ActionResult<Result> DeleteUserOutStore(string storeNo, string userId)
        {
            return Json(_accountRepository.DeleteUserOutStore(userId, storeNo));
        }
        public ActionResult<Result> AddUserToStore(string userId, string storeNos)
        {
            return Json(_accountRepository.AddUserToStore(userId, storeNos));
        }
        #endregion
    }
}
