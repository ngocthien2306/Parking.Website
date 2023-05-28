using AutoMapper;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Parking.Repositories.IRepo;
using Modules.Pleiger.CommonModels.Models;
using Modules.Pleiger.CommonModels.Parking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Modules.Parking.Controllers
{
    public class VehicleHistoryController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IAccessMenuService _accessMenuService;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IUserService _userService;
        private readonly IVehicleHistoryRepository _memberHistoryMgt;

        public VehicleHistoryController(IMapper mapper, IAccessMenuService accessMenuService, IHttpContextAccessor contextAccessor,
                                        IVehicleHistoryRepository memberHistoryMgt, IUserService userService):base(contextAccessor)
        {
            this.mapper = mapper;
            _accessMenuService = accessMenuService;
            this.contextAccessor = contextAccessor;
            _userService = userService;
            this._memberHistoryMgt = memberHistoryMgt;
        }

        //private readonly IKIOMemberManagement _memberManagement;
        #region View
        public IActionResult Index()
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
        #endregion
        #region Get Data
        public ActionResult<List<ParkingVehicleHistory>> GetMemberInVehicle(string storeNo, string userId, int lessMonth, int onceRecently)
        {
            var listMember = _memberHistoryMgt.GetMemberManagement(storeNo, userId, lessMonth, onceRecently);
            return Json(listMember);
        }
        [HttpGet]
        public IActionResult GetMemberInVehicleDetail(string storeNo,string userId,string lp, string ViewbagIndex, int MenuParent)
        {
            ViewBag.SAVE_YN = false;
            ViewBag.DELETE_YN = false;
            var ListButtonPermissionByUser = _accessMenuService.GetButtonPermissionByUser(CurrentUser.SiteID, MenuParent, CurrentUser.UserCode);
            if (ListButtonPermissionByUser.Count > 0)
            {
                ViewBag.SAVE_YN = ListButtonPermissionByUser[0].SAVE_YN;
                ViewBag.DELETE_YN = ListButtonPermissionByUser[0].DELETE_YN;
            }

            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.Index = ViewbagIndex;
            ViewBag.UserName = CurrentUser.UserName;
            var member = new ParkingHistoryDetail();
            if (storeNo != null)
            {   
                member = _memberHistoryMgt.GetMemberManagementDetail(storeNo, userId, lp).FirstOrDefault();
            }
            return PartialView("VehicleHistoryDetail", member);
        }
        public List<ParkingHistoryDetail> GetVehicleHistory(string storeNo, string userId, string lp)
        {
            var member = _memberHistoryMgt.GetMemberManagementDetail(storeNo, userId, lp);
            return member;
        }
        #endregion
    }
}
