using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using InfrastructureCore;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Kiosk.Monitoring.Repositories.IRepository;
using Modules.Pleiger.CommonModels.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static ClosedXML.Excel.XLPredefinedFormat;

namespace Modules.Kiosk.Monitoring.Controllers
{
    public class CheckInHistoryController : BaseController
    {

        private readonly IMapper mapper;
        private readonly IAccessMenuService _accessMenuService;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IKIOCheckInRepository _kIOCheckIn;

        private readonly IUserService _userService;
        public CheckInHistoryController(IMapper mapper,IKIOCheckInRepository kIOCheckIn,
        IAccessMenuService accessMenuService, IHttpContextAccessor contextAccessor, IUserService userService) : base(contextAccessor)
        {
            this._kIOCheckIn = kIOCheckIn;
            this.mapper = mapper;
            this._accessMenuService = accessMenuService;
            this.contextAccessor = contextAccessor;
            this._userService = userService;
        }

        #region View

        public ActionResult Index()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var curUrlTemp = (Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            var curMenu = CurrentUser != null ? CurrentUser.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;
            //var userStore = _kIOCheckIn.GetUserStoreMgt(null, CurrentUser.UserCode, null).SingleOrDefault();
            //if(userStore != null)
            //{
            //    ViewBag.StoreNo = userStore.storeNo;
            //}

            if(CurrentUser.UserType == "G000C001" || CurrentUser.UserType == "G000C002")
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
        public ActionResult<List<KIO_CheckInInfo>> GetCheckInInfo(string storeNo,string startDate, string endDate, int byMin, bool onlyUnknown)
        {
            var min = byMin;
            if (byMin == 61)
            {
                storeNo = null;
                min = 60;
            }



            var checkins = _kIOCheckIn.GetCheckInInfoUnknown(storeNo, null, null, min, onlyUnknown);
            return Json(checkins);
        }
        public IActionResult GetImageTaken(string hisNo)
        {
            try
            {
                var checkins = _kIOCheckIn.GetFacePhotoById(hisNo);
                if(checkins == null)
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
        public IActionResult GetImageCheckIn(string hisNo)
        {
            try
            {
                var checkins = _kIOCheckIn.GetCheckInPhotoById(hisNo);
                if (checkins == null)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads/images/user-empty.png");
                    return base.File(System.IO.File.ReadAllBytes(path), "image/jpeg");
                }
                return File(checkins.faceCheckIn, "image/jpeg");
            }

            catch
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads/images/user-empty.png");

                return base.File(System.IO.File.ReadAllBytes(path), "image/jpeg");
            }
        }
        public IActionResult GetImageCardId(string hisNo)
        {
            try
            {
                var checkins = _kIOCheckIn.GetCardIdPhotoById(hisNo);
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
