using InfrastructureCore;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Parking.Repositories.IRepo;
using Modules.Pleiger.CommonModels.Parking;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Modules.Parking.Controllers
{
    public class ParkingSiteController : BaseController
    {
        private readonly IAccessMenuService _accessMenuService;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IParkingRepository _parkingRepository;

        public ParkingSiteController(IAccessMenuService accessMenuService,
            IHttpContextAccessor contextAccessor,
            IParkingRepository parkingRepository) : base(contextAccessor)
        {
            _parkingRepository = parkingRepository;
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
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }
        #endregion

        #region Get Data
        [HttpGet] 
        public ActionResult<List<ParkingMaster>> GetListParkingMaster([FromRoute] string id)
        {
            return Json(_parkingRepository.GetListParkingMaster(id));
        }
        #endregion

        #region Create - Update -  Delete
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

        #endregion
    }
}
