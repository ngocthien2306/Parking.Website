using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Pleiger.FileUpload.Services.IService;
using Modules.Pleiger.MasterData.Services.IService;
using Modules.Pleiger.SalesProject.Services.IService;
using System;
using System.Data;
using System.Linq;



namespace Modules.Pleiger.SalesProject.Controllers
{
    public class MESTestController :  BaseController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUploadFileWithTemplateService _uploadFileWithTemplateService;
        private readonly IMESComCodeService _mesComCodeService;
        private readonly IMESItemService _mESItemService;
        private readonly IAccessMenuService _accessMenuService;
        private readonly IUserService _userService;
        private readonly IMESSaleOrderProjectService _mESSaleOrder;
        private static string EXCEL_TEMPLATE_NAME = "TemplateSOP.xlsx";

        public MESTestController(IHttpContextAccessor contextAccessor, IAccessMenuService accessMenuService,
            IUserService userService, IMESSaleOrderProjectService mESSaleOrder,
            IUploadFileWithTemplateService uploadFileWithTemplateService,
            IMESItemService mESItemService, IMESComCodeService mesComCodeService) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _accessMenuService = accessMenuService;
            _userService = userService;
            _mESSaleOrder = mESSaleOrder;
            _uploadFileWithTemplateService = uploadFileWithTemplateService;
            _mesComCodeService = mesComCodeService;
            _mESItemService = mESItemService;
        }
        public IActionResult Test()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.EXPORT_EXCEL_ICUBE_YN = false;
            ViewBag.CREATE_YN = false;
            ViewBag.SEARCH_YN = false;
            ViewBag.EXCEL_YN = false;

            var curUrlTemp = (Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            var curMenu = CurrentUser != null ? CurrentUser.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;

            // Quan add 2021-02-25        
            var ListButtonPermissionByUser = _accessMenuService.GetButtonPermissionByUser(CurrentUser.SiteID, curMenu.MenuID, CurrentUser.UserCode);
            if (ListButtonPermissionByUser.Count > 0)
            {
                ViewBag.EXPORT_EXCEL_ICUBE_YN = ListButtonPermissionByUser[0].EXPORT_EXCEL_ICUBE_YN;
                ViewBag.CREATE_YN = ListButtonPermissionByUser[0].CREATE_YN;
                ViewBag.SEARCH_YN = ListButtonPermissionByUser[0].SEARCH_YN;
                ViewBag.EXCEL_YN = ListButtonPermissionByUser[0].EXCEL_YN;
            }

            ViewBag.MenuId = curMenu != null ? curMenu.MenuID : 0;
            ViewBag.CurrentUser = CurrentUser;
            var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID);
            ViewBag.UserType = checkUserLogin.SystemUserType;
            ViewBag.UserCode = CurrentUser.UserCode;
            return View();
        }
    }
}
