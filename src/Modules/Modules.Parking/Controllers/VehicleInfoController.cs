using InfrastructureCore;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Parking.Helper;
using Modules.Parking.Repositories.IRepo;
using Modules.Pleiger.CommonModels.Parking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Modules.Parking.Controllers
{
    public class VehicleInfoController : BaseController
    {
        private readonly IAccessMenuService _accessMenuService;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IParkingRepository _parkingRepository;
        private readonly IVehicleHistoryRepository _vehicleHistoryRepository;

        public VehicleInfoController(IAccessMenuService accessMenuService,
            IVehicleHistoryRepository vehicleHistoryRepository,
            IHttpContextAccessor contextAccessor,
            IParkingRepository parkingRepository) : base(contextAccessor)
        {
            _parkingRepository = parkingRepository;
            _accessMenuService = accessMenuService;
            _vehicleHistoryRepository = vehicleHistoryRepository;
        }

        #region Views
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
            ViewBag.UserCode = CurrentUser.UserCode;
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }
        public IActionResult ShowVehicleDetail(string storeNo, string vehicleId, string lp, string viewbagIndex, int menuParent)
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
            ViewBag.UserType = CurrentUser.SystemUserType;
            ViewBag.UserCode = CurrentUser.UserCode;
            var vehiceInfo = new VehiceInfo();
            if (vehicleId != null)
            {
                vehiceInfo = _vehicleHistoryRepository.GetVehiceInfo(storeNo, CurrentUser.UserCode, lp, vehicleId).FirstOrDefault();
            }
            vehiceInfo.userId = CurrentUser.UserCode;
            return PartialView("ShowVehicleDetail", vehiceInfo);

        }
        #endregion

        #region Get Data
        [HttpGet]
        public ActionResult<object> GetImageVehicle(string userId, string vehicleId)
        {
            var vehiceInfo = _vehicleHistoryRepository.GetVehiceInfo(null, CurrentUser.UserCode, null, vehicleId).FirstOrDefault();

            try
            {
                if (vehiceInfo == null)
                {
                    vehiceInfo = new VehiceInfo();
                }
                var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads/images/user-empty.png");
                var noAvailableImage = System.IO.File.ReadAllBytes(path);
                vehiceInfo.vehiclePhotoBase64 = vehiceInfo.vehiclePhoto == null ? "data:image/png;base64," + Convert.ToBase64String(noAvailableImage) : "data:image/png;base64," + Convert.ToBase64String(vehiceInfo.vehiclePhoto);
                vehiceInfo.licensePhotoBase64 = vehiceInfo.licensePhoto == null ? "data:image/png;base64," + Convert.ToBase64String(noAvailableImage) : "data:image/png;base64," + Convert.ToBase64String(vehiceInfo.licensePhoto);
                return Json(vehiceInfo);
            }
            catch
            {
                return Json(vehiceInfo);
            }

        }

        [HttpGet]
        public ActionResult<List<VehiceInfo>> GetListVehicleInfo([FromRoute] string storeNo, string userId, string plateNum)
        {
            return Json(_vehicleHistoryRepository.GetVehiceInfo(storeNo, userId, plateNum, null));
        }
        [HttpGet] 
        public ActionResult<List<ParkingMaster>> GetListParkingMaster([FromRoute] string id)
        {
            return Json(_parkingRepository.GetListParkingMaster(id));
        }
        #endregion

        #region Create - Update -  Delete
        [HttpPost]
        public async Task<ActionResult<Result>> SaveVehicle(VehiceInfo vehice)
        {
            vehice.userId = CurrentUser.UserCode;
            var res = _vehicleHistoryRepository.SaveVehicle(vehice);
            if(res.Success)
            {
                string imageVehicle64 = Convert.ToBase64String(res.Data as byte[]);
                Result apiResponse = await ApiCaller.AddVehicleUserToFolder(vehice.userId, vehice.plateNum, imageVehicle64);
                if(apiResponse.Success)
                {
                    res.Success = true;
                }
                else
                {
                    res.Success = false;
                    res.Message = apiResponse.Message;
                }
            }
            return Json(res);
        }
        [HttpPost]
        public ActionResult<Result> SaveParkingSite([FromBody] ParkingMaster parkingMaster)
        {
            return Json(_parkingRepository.SaveParkingMaster(parkingMaster));
        }
        
        [HttpDelete]
        public ActionResult<Result> DeleteParkingSite([FromRoute] int id)
        {
            return Json(_parkingRepository.DeletParking(id));
        }
        
        [HttpDelete]
        public ActionResult<Result> DeleteVehicle(int vehicleId)
        {
            var res = _vehicleHistoryRepository.DeleteVehicle(vehicleId, CurrentUser.UserCode);
            return Json(res);
        }
        #endregion
    }
}
