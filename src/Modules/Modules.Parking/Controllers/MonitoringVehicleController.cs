using InfrastructureCore;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Kiosk.Monitoring.Repositories.IRepository;
using Modules.Parking.Repositories.IRepo;
using Modules.Pleiger.CommonModels.Parking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Modules.Parking.Controllers
{
    public class MonitoringVehicleController : BaseController
    {

        private readonly IAccessMenuService _accessMenuService;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IKIOCheckInRepository _kIOCheckIn;
        private readonly IVehicleCheckinRepository _vehicleCheckin;

        public MonitoringVehicleController(IAccessMenuService accessMenuService, IHttpContextAccessor contextAccessor, 
            IKIOCheckInRepository kIOCheckIn, IVehicleCheckinRepository vehicleCheckin) : base(contextAccessor)
        {
            _accessMenuService = accessMenuService;
            this.contextAccessor = contextAccessor;
            _kIOCheckIn = kIOCheckIn;
            _vehicleCheckin = vehicleCheckin;
        }


        #region View

        public ActionResult Index()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var curUrlTemp = (Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            var curMenu = CurrentUser != null ? CurrentUser.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;

            if (CurrentUser.UserType == "G000C001" || CurrentUser.UserType == "G000C002")
            {
                ViewBag.StoreNo = null;
            }
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
        [HttpGet]
        public ActionResult<List<ParkingCheckin>> GetVehicleCheckin(string startTime, string endTime, string byMin, string storeNo, string status)
        {
            var checkins = _vehicleCheckin.GetListVehicleCheckin(startTime, endTime, byMin, storeNo, status);
            return Json(checkins);
        }
        public IActionResult GetImageTaken(string userId)
        {
            try
            {
                var checkins = _kIOCheckIn.GetPhotoById(userId);
                if (checkins == null)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads/images/user-empty.png");
                    return base.File(System.IO.File.ReadAllBytes(path), "image/jpeg");
                }
                return File(checkins.takenPhoto, "image/jpeg");
            }

            catch
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads/images/user-empty.png");
                return base.File(System.IO.File.ReadAllBytes(path), "image/jpeg");
            }
        }
        public IActionResult GetImageCardId(string userId)
        {
            try
            {
                var checkins = _kIOCheckIn.GetPhotoById(userId);
                if (checkins == null)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads/images/user-empty.png");
                    return base.File(System.IO.File.ReadAllBytes(path), "image/jpeg");
                }
                return File(checkins.idCardPhoto, "image/jpeg");
            }

            catch
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads/images/user-empty.png");

                return base.File(System.IO.File.ReadAllBytes(path), "image/jpeg");
            }
        }
        #endregion

        #region Create - Update - Delete
        [HttpGet]
        public ActionResult<Result> UpdateApproveRejectUser(string userId, bool status)
        {
            return Json(_kIOCheckIn.UpdateApproveRejectUser(userId, status));
        }
        [HttpGet]
        public ActionResult<Result> UpdateApproveRejectRemoveUser(string userId, bool status)
        {
            return Json(_kIOCheckIn.UpdateApproveRejectRemoveUser(userId, status));
        }
        #endregion

    }
}
